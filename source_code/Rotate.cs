using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aircraft
{
    public class Rotate : MonoBehaviour
    {
        [Tooltip("The speed at which to rotate")]
        public Vector3 rotateSpeed;

        [Tooltip("Whether to randomize the start position")]
        public bool randomize = false;

        // Start is called before the first frame update
        void Start()
        {
            // Randomize the start position
            if (randomize) transform.Rotate(rotateSpeed.normalized * UnityEngine.Random.Range(0f, 360f));
        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(rotateSpeed * Time.deltaTime, Space.Self);
        }
    }
}
