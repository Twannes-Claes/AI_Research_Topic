using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Aircraft
{
    public class AircraftPlayer : AircraftAgent
    {
        public InputAction pitchInput;
        public InputAction yawInput;
        //public InputAction rollInput;

        //public InputAction accelerateInput;
        //public InputAction descelareteInput;

        public InputAction boostInput;
        public InputAction pauzeInput;

        public InputAction up;
        public InputAction down;

        public List<Transform> camLocations;

        public Transform playerCam;

        public int currCam = 0;

        public CinemachineVirtualCamera camera;

        public float tempValueUp;
        public float tempValueDown;

        public bool isPlayer;

        public override void Initialize()
        {
            base.Initialize();

            

            //enabling the input otherwise it doesnt register
            pitchInput.Enable();
            yawInput.Enable();
            //rollInput.Enable();

            //accelerateInput.Enable();
            //descelareteInput.Enable();

            up.Enable();
            down.Enable();

            boostInput.Enable();
            pauzeInput.Enable();
        }

        public void Update()
        {
            float upValue = Mathf.Round(up.ReadValue<float>());

            float downValue = Mathf.Round(down.ReadValue<float>());

            if (upValue == 1f && tempValueUp != 1f)
            {
                isPlayer = false;
                currCam = (currCam + 1) % camLocations.Count;

                camera.Follow = camLocations[currCam];
                camera.LookAt = camLocations[currCam];
            }

            if (downValue == 1f && tempValueDown != 1f)
            {
                isPlayer = true;
                camera.Follow = playerCam;
                camera.LookAt = playerCam;

            }

            tempValueUp = upValue;
            tempValueDown= downValue;

            if(isPlayer == false)
            {
                rigidbody.velocity = Vector3.zero;
            }

        }



        public override void Heuristic(float[] actionsOut)
        {

            //reading the values from the inputsw
            float pitchValue = Mathf.Round(pitchInput.ReadValue<float>());

            if (pitchValue == -1f) pitchValue = 2f;

            float yawValue = Mathf.Round(yawInput.ReadValue<float>());

            if (yawValue == -1f) yawValue = 2f;

            //float rollValue = Mathf.Round(rollInput.ReadValue<float>());

            //if (rollValue == -1f) rollValue = 2f;

            float boostValue = Mathf.Round(boostInput.ReadValue<float>());

            //float accelerate = Mathf.Round(accelerateInput.ReadValue<float>()); ;
            //float descelerate = Mathf.Round(descelareteInput.ReadValue<float>());

            //Debug.Log(pitchValue);

            //inserting the values in the right direction
            actionsOut[0] = pitchValue;
            actionsOut[1] = yawValue;
            //actionsOut[2] = rollValue;

            actionsOut[2] = boostValue;

            //actionsOut[3] = accelerate;
            //actionsOut[4] = descelerate;
        }

        private void OnDestroy()
        {
            //disable inputs on destroy
            pitchInput.Disable();
            yawInput.Disable();
            //rollInput.Disable();
            boostInput.Disable();
            pauzeInput.Disable();

            up.Disable();
            down.Disable();

            //accelerateInput.Disable();
            //descelareteInput.Disable();
        }
    }
}

