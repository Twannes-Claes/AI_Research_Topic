using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aircraft
{
    public class AircraftAgent : Agent
    {
        public float thrust = 100000f;
        public float pitchSpeed = 100f;
        public float yawSpeed = 100f;
        public float rollSpeed = 100f;
        public float boostMultiplier = 2f;

        new private Rigidbody rigidbody;
        private TrailRenderer trail;

        public int stepTimeOut = 1000;
        private float nextStepTimeOut;

        private bool frozen = false;

        public GameObject meshObject;
        public GameObject explosionEffect;

        private float pitchChange = 0f;
        private float smoothPitchChange = 0f;
        private float maxPitchAngle = 45;

        private float yawChange = 0f;
        private float smoothYawChange = 0f;

        private float rollChange = 0f;
        private float smoothRollChange = 0f;
        private float maxRollAngle = 45f;

        private bool boost;

        public override void Initialize()
        {
            rigidbody= GetComponent<Rigidbody>();
            trail= GetComponent<TrailRenderer>();
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            pitchChange = vectorAction[0];
            if (pitchChange == 2) pitchChange = -1f;

            yawChange = vectorAction[1];
            if(yawChange == 2) yawChange = -1f;

            boost = vectorAction[2] == 1;
            if(boost && !trail.emitting) { trail.Clear(); }

            trail.emitting= boost;

            ProcessMovement();

        }

        private void ProcessMovement()
        {
            float boostModifier = boost ? boostMultiplier: 1f;

            rigidbody.AddForce(transform.forward * thrust * boostModifier, ForceMode.Force);

            Vector3 currRot = transform.rotation.eulerAngles;

            float rollAngle = currRot.z > 180f ? currRot.z - 360f : currRot.z;

            if(yawChange == 0)
            {
                rollChange = -rollAngle / maxRollAngle;
            }
            else
            {
                rollChange = -yawChange;
            }

            smoothPitchChange = Mathf.MoveTowards(smoothPitchChange, pitchChange, 2f * Time.fixedDeltaTime);
            smoothYawChange = Mathf.MoveTowards(smoothYawChange, yawChange, 2f * Time.fixedDeltaTime);
            smoothRollChange = Mathf.MoveTowards(smoothRollChange, rollChange, 2f * Time.fixedDeltaTime);


            float pitch = currRot.x + smoothPitchChange* Time.fixedDeltaTime * pitchSpeed;
            if (pitch > 180f) pitch -= 360f;

            pitch = Mathf.Clamp(pitch, -maxPitchAngle, maxPitchAngle);

            float yaw  = currRot.y + smoothYawChange* Time.fixedDeltaTime * yawSpeed;

            float roll = currRot.z + smoothRollChange * Time.fixedDeltaTime * rollSpeed;
            if (roll > 180f) roll -= 360f;

            roll = Mathf.Clamp(roll, -maxRollAngle, maxRollAngle);

            transform.rotation = Quaternion.Euler(pitch, yaw, roll);

        }
    }
}


