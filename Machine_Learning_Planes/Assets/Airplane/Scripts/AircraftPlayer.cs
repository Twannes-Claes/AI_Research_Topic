using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Aircraft
{
    public class AircraftPlayer : AircraftAgent
    {
        public InputAction pitchInput;
        public InputAction yawInput;
        public InputAction rollInput;

        public InputAction accelerateInput;
        public InputAction descelareteInput;

        public InputAction boostInput;
        public InputAction pauzeInput;

        public override void Initialize()
        {
            base.Initialize();

            //enabling the input otherwise it doesnt register
            pitchInput.Enable();
            yawInput.Enable();
            rollInput.Enable();

            accelerateInput.Enable();
            descelareteInput.Enable();

            boostInput.Enable();
            pauzeInput.Enable();
        }

        public override void Heuristic(float[] actionsOut)
        {

            //reading the values from the inputsw
            float pitchValue = Mathf.Round(pitchInput.ReadValue<float>());

            if (pitchValue == -1) pitchValue = 2;

            float yawValue = Mathf.Round(yawInput.ReadValue<float>());

            if (yawValue == -1) yawValue = 2;

            float rollValue = Mathf.Round(rollInput.ReadValue<float>());

            if (rollValue == -1) rollValue = 2;

            float boostValue = Mathf.Round(boostInput.ReadValue<float>());

            float accelerate = Mathf.Round(accelerateInput.ReadValue<float>()); ;
            float descelerate = Mathf.Round(descelareteInput.ReadValue<float>());

            //Debug.Log(pitchValue);

            //inserting the values in the right direction
            actionsOut[0] = pitchValue;
            actionsOut[1] = yawValue;
            actionsOut[2] = rollValue;

            actionsOut[3] = boostValue;

            actionsOut[4] = accelerate;
            actionsOut[5] = descelerate;
        }

        private void OnDestroy()
        {
            //disable inputs on destroy
            pitchInput.Disable();
            yawInput.Disable();
            rollInput.Disable();
            boostInput.Disable();
            pauzeInput.Disable();

            accelerateInput.Disable();
            descelareteInput.Disable();
        }
    }
}

