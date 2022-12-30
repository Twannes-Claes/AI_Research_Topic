using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aircraft
{
    public class Rotate : MonoBehaviour
    {

        public Vector3 rotateSpeed;

        public bool randomize = false;

        // Start is called before the first frame update
        void Start()
        {
            if(randomize) { transform.Rotate(rotateSpeed.normalized * UnityEngine.Random.Range(0f, 360f)); }
        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(rotateSpeed * Time.deltaTime, Space.Self);
        }
    }
}


