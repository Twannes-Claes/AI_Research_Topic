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
        public InputAction boostInput;
        public InputAction pauzeInput;

        public override void Initialize()
        {
            base.Initialize();

            pitchInput.Enable();
              yawInput.Enable();
             rollInput.Enable();
            boostInput.Enable();
            pauzeInput.Enable();
        }

        public override void Heuristic(float[] actionsOut)
        {
            float pitchValue = Mathf.Round(pitchInput.ReadValue<float>());

            float yawValue = Mathf.Round(yawInput.ReadValue<float>());

            //float rollValue = Mathf.Round(rollInput.ReadValue<float>());

            float boostValue = Mathf.Round(boostInput.ReadValue<float>());

            if (pitchValue == -1) pitchValue = 2f;

            if (yawValue == -1) yawValue = 2f;

            actionsOut[0] = pitchValue;
            actionsOut[1] = yawValue;
            actionsOut[2] = boostValue;
        }

        private void OnDestroy()
        {
            pitchInput.Disable();
              yawInput.Disable();
             rollInput.Disable();
            boostInput.Disable();
            pauzeInput.Disable();
        }
    }
}

