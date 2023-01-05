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
        //private float throttleIncrement = 0.5f;
        //private float throttle = 5f;

        // private float responsiveness = 5f;

        //private float pitch;
        // private float yaw;
        //private float roll;

        // private float responseModifier;

        //bool isAccelerating = false;
        //bool isDescelerating = false;

        public float thrust = 1f;
        public float pitchSpeed = 50f;
        public float yawSpeed = 50f;
        public float rollSpeed = 50f;

        private float pitchChange = 0f;
        private float smoothPitchChange = 0f;
        private float maxPitchAngle = 45f;
        private float yawChange = 0f;
        private float smoothYawChange = 0f;
        private float rollChange = 0f;
        private float smoothRollChange = 0f;
        private float maxRollAngle = 45;

        private bool boost;

        new protected Rigidbody rigidbody;
        private TrailRenderer trail;
        private AircraftArea area;

        public Transform propellor;

        public int stepTimeOut = 300;
        private float nextStepTimeOut;

        private bool frozen = false;

        public GameObject meshObject;
        public GameObject explosionEffect;

        public int nextCheckPointIndex { get; set; }

        public override void Initialize()
        {
            rigidbody= GetComponent<Rigidbody>();
            trail= GetComponent<TrailRenderer>();
            area = GetComponentInParent<AircraftArea>();

            if (rigidbody == null) return;

            //logic for plane rotating
            //responseModifier = rigidbody.mass / 10f * responsiveness;

            //when not training infinite steps
            MaxStep = area.isTraining ? 5000 : 0;

        }

        //called when training starts
        public override void OnEpisodeBegin()
        {

            //reset basic transform
            rigidbody.velocity = Vector3.zero;

            rigidbody.angularVelocity= Vector3.zero;

            trail.emitting = false;

            //throttle = 5;

            area.ResetAgentPosition(this, area.isTraining);

            if (area.isTraining) nextStepTimeOut = StepCount + stepTimeOut;

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
            pitchChange = vectorAction[0];

            if (pitchChange == 2f) pitchChange = -1f;

            yawChange = vectorAction[1];

            if (yawChange == 2f) yawChange = -1f;

            boost = vectorAction[2] == 1;

            //trail when boosting
            if (boost && !trail.emitting) { trail.Clear(); }

            trail.emitting = boost;

            ProcessMovement();

            if(area.isTraining)
            {
                //negative reward every step

                AddReward(-1f / MaxStep);

                /*if(throttle < 5f)
                {
                    AddReward(-0.01f);
                    //Debug.Log("To slow");
                }*/

                //check if time is up

                if (StepCount > nextStepTimeOut)
                {
                    AddReward(-0.5f);
                    EndEpisode();
                    //Debug.Log("Out of steps");
                }

                Vector3 localCheckpointDir = VectorToNextCheckpoint();

                if (localCheckpointDir.magnitude < Academy.Instance.EnvironmentParameters.GetWithDefault("checkpoint_radius", 0))
                {

                    GotCheckpoint();
                    Debug.Log("Got it by the file");
                    //Debug.Log("In range");

                }

                #region owncode
                /*Vector3 dirToPlayer = (currentPlayerTransform.position - transform.position).normalized;

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
                }*/
                #endregion
            }

        }

        private Vector3 VectorToNextCheckpoint()
        {
            Vector3 nextDir = area.checkPoints[nextCheckPointIndex].transform.position - transform.position;

            return transform.InverseTransformDirection(nextDir);
        }

        private void GotCheckpoint()
        {
            nextCheckPointIndex = (nextCheckPointIndex + 1) % area.checkPoints.Count;

            if (!area.isTraining) return;

            AddReward(0.5f);
            nextStepTimeOut = StepCount + stepTimeOut;
            Debug.Log("got one reward");
        }

        ////let the ai observe his own actions
        public override void CollectObservations(VectorSensor sensor)
        {
            //observe velocity (vector3 = 3 values)
            sensor.AddObservation(transform.InverseTransformDirection(rigidbody.velocity));
        
            //where is nextcheckpoint (vector3 = 3 values)
            sensor.AddObservation(VectorToNextCheckpoint());
        
            //orientation of pcheckpoint ( vector 3 = 3 values)
            Vector3 checkForward = area.checkPoints[nextCheckPointIndex].transform.forward;
            sensor.AddObservation(transform.InverseTransformDirection(checkForward));
        
            //total observations = 3 * 3 = 9
        
        }
        
        public override void Heuristic(float[] actionsOut)
        {
            Debug.LogError("Heuristic was called on " + gameObject.name + " make sure only player is set to behaviour type heuristic only");
        }

        //no actions and movement when freezed
        public void FreezeAgent()
        {
            Debug.Assert(area.isTraining == false, "Freeze not supported when not training");
            frozen = true;
            rigidbody.Sleep();
            trail.emitting = false;
        }

        //enable it again
        public void ThawAgent()
        {
            Debug.Assert(area.isTraining == false, "Thaw not supported when not training");
            frozen = false;
            //throttle = 0;
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

            //caculate the boost multiplier
            float boostMulti = boost == true ? 1.25f : 1f;

            rigidbody.velocity = transform.forward * 75 * boostMulti;

            //current rotation
            Vector3 currRot = transform.rotation.eulerAngles;

            //calc roll angle
            float rollAngle = currRot.z > 180f ? currRot.z - 360f : currRot.z;
            
            if(yawChange == 0f)
            {
                rollChange = -rollAngle / maxRollAngle;
            }
            else
            {
                rollChange = -yawChange;
            }

            //smooth the angles
            smoothPitchChange = Mathf.MoveTowards(smoothPitchChange, pitchChange, 2f * Time.fixedDeltaTime);
            smoothYawChange = Mathf.MoveTowards(smoothYawChange, yawChange, 2f * Time.fixedDeltaTime);
            smoothRollChange = Mathf.MoveTowards(smoothRollChange, rollChange, 2f * Time.fixedDeltaTime);

            //now calculate the rotations

            float pitch = currRot.x + smoothPitchChange * Time.fixedDeltaTime * pitchSpeed;
            if (pitch > 180f)  pitch -= 360f;
            pitch = Mathf.Clamp(pitch, - maxPitchAngle, maxPitchAngle);

            float yaw = currRot.y + smoothYawChange * Time.fixedDeltaTime * yawSpeed;

            float roll = currRot.z + smoothRollChange * Time.fixedDeltaTime * rollSpeed;
            if (roll > 180f) roll -= 360f;
            roll = Mathf.Clamp(roll, -maxRollAngle, maxRollAngle);

            transform.rotation = Quaternion.Euler(pitch, yaw, roll);

            //Debug.Log(throttle);
            #region old
            /*//change velocity based on speed and rotation
            rigidbody.velocity = transform.forward * throttle * boostMulti;

            Debug.Log(rigidbody.velocity);

            //add angular speed
            if (pitch != 0) rigidbody.AddTorque(transform.right * pitch * responseModifier);
            if (  yaw != 0) rigidbody.AddTorque(transform.up * yaw * responseModifier);
            //if ( roll != 0) rigidbody.AddTorque(-transform.forward * roll * responseModifier);

            //rotate the propellor based on its speed for simple visualization

            propellor.Rotate(Vector3.forward * throttle);*/
            #endregion

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("checkpoint") && other.gameObject == area.checkPoints[nextCheckPointIndex])
            {
                GotCheckpoint();
                Debug.Log("hello checky");
            }
            if(other.transform.CompareTag("level"))
            {
                if (area.isTraining)
                {
                    AddReward(-1f);
                    EndEpisode();
                }
                else
                {
                    //Debug.Log("i did hit??");
                    StartCoroutine(ExplosionReset());
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {


            if(!collision.transform.CompareTag("agent"))
            {
                //hit something that isnt an agent

                
            }
            
        }

        private IEnumerator ExplosionReset()
        {
            FreezeAgent();

            meshObject.SetActive(false);
            explosionEffect.SetActive(true);
            yield return new WaitForSeconds(2f);

            explosionEffect.SetActive(false);
            //Debug.Log("I exploded");
            meshObject.SetActive(true);

            area.ResetAgentPosition(this);
            yield return new WaitForSeconds(1f);

            ThawAgent();
        }

        

    }
}


