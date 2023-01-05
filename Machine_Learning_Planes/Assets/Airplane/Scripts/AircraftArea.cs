using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Aircraft
{
    public class AircraftArea : MonoBehaviour
    {

        public CinemachineSmoothPath racePath;

        public GameObject checkpointPrefab;

        public GameObject finishPrefab;

        public bool isTraining;

        public List<AircraftAgent> aircraftAgents { get; private set; }

        public List<GameObject> checkPoints { get; private set; }

        private void Awake()
        {
            if( aircraftAgents == null) FindAircraftAgents();

        }

        

        private void Start()
        {
            if( checkPoints == null) CreateCheckPoints();

        }

        private void CreateCheckPoints()
        {
            checkPoints = new List<GameObject>();

            //get all the checkpoints
            int numPoints = (int)racePath.MaxUnit(CinemachinePathBase.PositionUnits.PathUnits);

            for (int i = 0; i < numPoints; i++)
            {
                GameObject checkPoint;

                if (i == numPoints - 1)
                {
                    //if is last one make it the finish checkpoints
                    checkPoint = Instantiate<GameObject>(finishPrefab);
                }
                else
                {
                    //else a normal checkpoint
                    checkPoint = Instantiate<GameObject>(checkpointPrefab);
                }

                //set the transform of the checkpoint and add it to the list
                checkPoint.transform.SetParent(racePath.transform);
                checkPoint.transform.localPosition = racePath.m_Waypoints[i].position;
                checkPoint.transform.rotation = racePath.EvaluateOrientationAtUnit(i, CinemachinePathBase.PositionUnits.PathUnits);

                checkPoints.Add(checkPoint);

            }
        }

        private void FindAircraftAgents()
        {
            //get all agents that are part of the race
            aircraftAgents = transform.GetComponentsInChildren<AircraftAgent>().ToList();
        }

        public void ResetAgentPosition(AircraftAgent agent, bool randomize = false)
        {

            if (aircraftAgents == null) FindAircraftAgents();

            if (checkPoints == null) CreateCheckPoints();

            if (randomize)
            {
                //random checkpoint
                agent.nextCheckPointIndex = Random.Range(0, checkPoints.Count);

            }

            //set position to previous checkpoint

            int previousCheckPointIndex = agent.nextCheckPointIndex -1;

            if(previousCheckPointIndex == -1) previousCheckPointIndex = checkPoints.Count -1;

            float startPostion = racePath.FromPathNativeUnits(previousCheckPointIndex, CinemachinePathBase.PositionUnits.PathUnits);

            //convert to 3d space

            Vector3 basePosition = racePath.EvaluatePosition(startPostion);

            //get rotation

            Quaternion rotation = racePath.EvaluateOrientation(startPostion);

            //calculate offset for the multiple agents to avoid collision

            Vector3 offset = Vector3.right * (aircraftAgents.IndexOf(agent) - aircraftAgents.Count / 2f) * Random.Range(9f, 10f);

            //set postion and rotation

            agent.transform.position = basePosition + rotation * offset;
            agent.transform.rotation = rotation;

        }

    }
}



