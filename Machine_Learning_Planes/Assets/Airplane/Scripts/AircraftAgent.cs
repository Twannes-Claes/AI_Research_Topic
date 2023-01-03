using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aircraft
{
    public class AircraftAgent : Agent
    {
        private float thrust = 100000f;
        private float pitchSpeed = 100f;
        private float yawSpeed = 100f;
        private float rollSpeed = 100f;
        private float boostMultiplier = 2f;

        private float throttleIncrement = 0.5f;
        private float maxThrottle = 200f;
        
        private float responsiveness = 5f;

        private float throttle;
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

        public int stepTimeOut = 1000;
        private float nextStepTimeOut;

        private bool frozen = false;

        public GameObject meshObject;
        public GameObject explosionEffect;

       // private float pitchChange = 0f;
        //private float smoothPitchChange = 0f;
        //private float maxPitchAngle = 45;

        //private float yawChange = 0f;
        //private float smoothYawChange = 0f;

       // private float rollChange = 0f;
        //private float smoothRollChange = 0f;
       // private float maxRollAngle = 45f;


        public override void Initialize()
        {
            rigidbody= GetComponent<Rigidbody>();
            trail= GetComponent<TrailRenderer>();

            if (rigidbody == null) return;

            responseModifier = rigidbody.mass / 10f * responsiveness;
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            pitch = vectorAction[0];

            yaw = vectorAction[1];

            roll = vectorAction[2];

            roll *= -1;

            boost = vectorAction[3] == 1;

            if(boost && !trail.emitting) { trail.Clear(); }

            trail.emitting = boost;

            isAccelerating = (vectorAction[4] == 1 && vectorAction[5] == 0);

            isDescelerating= vectorAction[5] == 1; 

            ProcessMovement();

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

            if (isAccelerating) throttle += throttleIncrement;

            if(isDescelerating) throttle -= throttleIncrement;

            throttle = Mathf.Clamp(throttle, 15f, 100f);

            float boostMulti = boost == true ? 1.2f : 1f;

            Debug.Log(throttle);

            rigidbody.velocity = transform.forward * throttle * boostMulti;

            rigidbody.AddTorque(transform.up * yaw * responseModifier);
            rigidbody.AddTorque(transform.right * pitch * responseModifier);
            rigidbody.AddTorque(-transform.forward * roll * responseModifier);

            propellor.Rotate(Vector3.forward * throttle);

        }

    }
}


