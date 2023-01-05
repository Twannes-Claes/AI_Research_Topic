using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

namespace Aircraft
{
    public class AircraftAgent : Agent
    {
        private float throttleIncrement = 0.5f;
        private float throttle;
        
        private float responsiveness = 5f;

        private float pitch;
        private float yaw;
        private float roll;

        private float responseModifier;

        bool isAccelerating = false;
        bool isDescelerating = false;

        private bool boost;

        new private Rigidbody rigidbody;
        private TrailRenderer trail;

        public Transform propellor;
        public Transform stabilizer;

        public int stepTimeOut = 50000;
        private float nextStepTimeOut;

        private bool frozen = false;

        public GameObject meshObject;
        public GameObject explosionEffect;

        private Vector3 originalPos;

        public bool isTraining;

        private Vector3 startPlayerPosition;
        private Quaternion startPlayerRotation;
        private Transform currentPlayerTransform;

        private int playerRadius = 250000;

        public override void Initialize()
        {
            rigidbody= GetComponent<Rigidbody>();
            trail= GetComponent<TrailRenderer>();

            if (rigidbody == null) return;

            //logic for plane rotating
            responseModifier = rigidbody.mass / 10f * responsiveness;

            //storing original position to reset the player
            originalPos = transform.position;

            //when not training infinite steps
            MaxStep = isTraining? 5000 : 0;

            currentPlayerTransform = FindObjectOfType<AircraftPlayer>().transform;

            startPlayerPosition = transform.position;
            startPlayerRotation = transform.rotation;

        }

        //called when training starts
        public override void OnEpisodeBegin()
        {

            //reset basic transform
            rigidbody.velocity = Vector3.zero;

            rigidbody.angularVelocity= Vector3.zero;

            trail.emitting = false;

            throttle = 0;

            rigidbody.position = startPlayerPosition;
            rigidbody.rotation = startPlayerRotation;

            if (isTraining) nextStepTimeOut = StepCount + stepTimeOut;

        }

        public override void OnActionReceived(float[] vectorAction)
        {
            //when dead no input
            if (frozen)
            {
                //Debug.Log("Im frozen");
                return;
            }

            //process the input from the heuristic
            pitch = vectorAction[0];

            if (pitch == 2) pitch = -1;

            yaw = vectorAction[1];

            if (yaw == 2) yaw = -1;

            roll = vectorAction[2];

            if (roll == 2) roll = -1;

            //roll *= -1;

            boost = vectorAction[3] == 1;

            //when only up arrow is pressed accelerate
            isAccelerating = (vectorAction[4] == 1 && vectorAction[5] == 0);

            isDescelerating= vectorAction[5] == 1;

            //trail when boosting
            if (boost && !trail.emitting) { trail.Clear(); }

            trail.emitting = boost;


            ProcessMovement();

            if(isTraining)
            {
                //negative reward every step

                AddReward(-1f / MaxStep);

                if(throttle < 75f)
                {
                    AddReward(-0.1f);
                    //Debug.Log("To slow");
                }

                //check if time is up

                if(StepCount > nextStepTimeOut)
                {
                    AddReward(-0.5f);
                    EndEpisode();
                    //Debug.Log("Out of steps");
                }

                Vector3 localPlayerDirection = currentPlayerTransform.position - transform.position;

                if (localPlayerDirection.sqrMagnitude < playerRadius)
                {

                    
                    AddReward(0.5f);
                    nextStepTimeOut = StepCount + stepTimeOut;
                    //Debug.Log("In range");


                          

                }

                Vector3 dirToPlayer = (currentPlayerTransform.position - transform.position).normalized;

                float dotprod = Vector3.Dot(dirToPlayer, transform.forward);

                bool isFacing = dotprod >= 0.9f;

                if(!isFacing) 
                {
                    AddReward(-0.1f);
                    //Debug.Log("not facing");

                    
                }
                else if(isFacing)
                {
                    AddReward(0.3f);
                    nextStepTimeOut += 50;
                    //Debug.Log("facing");
                }

            }

        }

        ////let the ai observe his own actions
        public override void CollectObservations(VectorSensor sensor)
        {
            //observe velocity (vector3 = 3 values)
            sensor.AddObservation(transform.InverseTransformDirection(rigidbody.velocity));
        
            //where is player (vector3 = 3 values)
            Vector3 localPlayerDirection = currentPlayerTransform.position - transform.position;
            sensor.AddObservation(localPlayerDirection);
        
            //orientation of player ( vector 3 = 3 values)
            Vector3 playerForward = currentPlayerTransform.forward;
            sensor.AddObservation(transform.InverseTransformDirection(playerForward));
        
            //total observations = 3 * 3 = 9
        
        }
        
        public override void Heuristic(float[] actionsOut)
        {
            Debug.LogError("Heuristic was called on " + gameObject.name + " make sure only player is set to behaviour type heuristic only");
        }

        //no actions and movement when freezed
        public void FreezeAgent()
        {
            Debug.Assert(isTraining == false, "Freeze not supported when not training");
            frozen = true;
            rigidbody.Sleep();
            trail.emitting = false;
        }

        //enable it again
        public void ThawAgent()
        {
            Debug.Assert(isTraining == false, "Thaw not supported when not training");
            frozen = false;
            throttle = 0;
            rigidbody.WakeUp();
        }

        private void ProcessMovement()
        {
            #region
            /*float boostModifier = boost ? boostMultiplier: 1f;

            rigidbody.AddForce(transform.forward * thrust * boostModifier, ForceMode.Force);

            Vector3 currRot = transform.localEulerAngles;

            smoothPitchChange = Mathf.MoveTowards(smoothPitchChange, pitchChange, 2f * Time.fixedDeltaTime);
            //smoothYawChange = Mathf.MoveTowards(smoothYawChange, yawChange, 2f * Time.fixedDeltaTime);
            smoothRollChange = Mathf.MoveTowards(smoothRollChange, rollChange, 2f * Time.fixedDeltaTime);


            float pitch = currRot.x + smoothPitchChange * Time.fixedDeltaTime * pitchSpeed;
            if (pitch > 180f) pitch -= 180f;

            //pitch = Mathf.Clamp(pitch, -maxPitchAngle, maxPitchAngle);

            //float yaw  = currRot.y + smoothYawChange* Time.fixedDeltaTime * yawSpeed;

            float roll = currRot.z + smoothRollChange * Time.fixedDeltaTime * rollSpeed;
            if (roll > 180f) roll -= 180f;

            //roll = Mathf.Clamp(roll, -maxRollAngle, maxRollAngle);

            transform.localRotation = Quaternion.Euler(pitch, 0, roll);*/
            #endregion

            //here i handle the plane movement
            //took some time to refine
            if (isAccelerating) throttle += throttleIncrement;

            if(isDescelerating) throttle -= throttleIncrement;

            throttle = Mathf.Clamp(throttle, 0f, 100f);

            //caculate the boost multiplier
            float boostMulti = boost == true ? 1.2f : 1f;

            //Debug.Log(throttle);

            //change velocity based on speed and rotation
            rigidbody.velocity = transform.forward * throttle * boostMulti;

            //add angular speed
            if (pitch != 0) rigidbody.AddTorque(transform.right * pitch * responseModifier);
            if (  yaw != 0) rigidbody.AddTorque(transform.up * yaw * responseModifier);
            if ( roll != 0) rigidbody.AddTorque(-transform.forward * roll * responseModifier);

            //rotate the propellor based on its speed for simple visualization
            propellor.Rotate(Vector3.forward * throttle);

        }

        private void OnCollisionEnter(Collision collision)
        {


            if(!collision.transform.CompareTag("agent"))
            {
                //hit something that isnt an agent
                if(collision.transform.CompareTag("level"))
                {
                    //Debug.Log("hitting level");

                }

                if (isTraining)
                {
                    AddReward(-100f);
                    EndEpisode();
                }
                else
                {
                    //Debug.Log("i did hit??");
                    StartCoroutine(ExplosionReset());
                }
            }
            
        }

        private IEnumerator ExplosionReset()
        {
            FreezeAgent();

            meshObject.SetActive(false);
            explosionEffect.SetActive(true);
            yield return new WaitForSeconds(2f);

            explosionEffect.SetActive(false);
            meshObject.SetActive(true);

            transform.position = startPlayerPosition;
            transform.rotation = quaternion.Euler(Vector3.zero);
            yield return new WaitForSeconds(1f);

            ThawAgent();
        }

    }
}


