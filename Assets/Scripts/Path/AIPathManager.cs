using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class AIPathManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private Transform[] lanes;
    [SerializeField] private int totalLanes = 3;
    [SerializeField] private float safeDistance = 20f;

    [SerializeField] private float pathTimerMinLimit = 6f;
    [SerializeField] private float pathTimerMaxLimit = 10f;


    private float pathTimer = 0;
    private float minDistance = 0;
    private float maxDistance = 5;
    private float pathTimerLimit = 5f;

    private bool canCreatePath;
    private bool isInitialSpawn = true;

    private int currentTrackLaneIdx = -1;

    private Vector3 globalEndPointPos = Vector3.zero;
    private Vector3 globalEndPointPosMax = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
    private Vector3 laneSpawnStartPos;

    private AIController aiController;
    private ObstaclesManager obstaclesManager;

    private Dictionary<int, ObstacleBase> lastSpawnedObstaclesInLane = new Dictionary<int, ObstacleBase>(); // convert to a list if required

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<AIPathManager>(this);
    }

    public void InitializeData()
    {
        aiController = InterfaceManager.Instance?.GetInterfaceInstance<AIController>();
        obstaclesManager = InterfaceManager.Instance?.GetInterfaceInstance<ObstaclesManager>();
    }

    public void SpawnObstacles()
    {
        int closerEndpointLaneIdx = 0;
        int safeAreaIdx = 0;

        if (isInitialSpawn)
        {
            isInitialSpawn = false;

            float distZ = 0, extraOffsetDist = 0;
            safeAreaIdx = Random.Range(0, totalLanes);

            for (int i = 0; i < totalLanes; i++)
            {
                if (i == safeAreaIdx)
                {
                    continue;
                }

                SetObstaclePositionData(out distZ, out extraOffsetDist, i);
            }

            Transform emptyLane = GetEmptyLane(safeAreaIdx);
            closerEndpointLaneIdx = FindCloserEndpointLaneIdx(safeAreaIdx);
        }
        else
        {
            List<int> laneIndexes = new List<int>();

            laneIndexes.Add(safeAreaIdx);
            if (safeAreaIdx != 0 && safeAreaIdx != totalLanes - 1)
            {
                Debug.Log($"safeAreaIdx: {safeAreaIdx}");
                if (lastSpawnedObstaclesInLane.ContainsKey(safeAreaIdx - 1) &&lastSpawnedObstaclesInLane[safeAreaIdx - 1].HasAIPassed)
                    laneIndexes.Add(safeAreaIdx - 1);

                if (lastSpawnedObstaclesInLane.ContainsKey(safeAreaIdx + 1) && lastSpawnedObstaclesInLane[safeAreaIdx + 1].HasAIPassed)
                    laneIndexes.Add(safeAreaIdx + 1);
            }
            else if (safeAreaIdx == 0)
            {
                if (lastSpawnedObstaclesInLane.ContainsKey(safeAreaIdx + 1) &&lastSpawnedObstaclesInLane[safeAreaIdx + 1].HasAIPassed)
                    laneIndexes.Add(safeAreaIdx + 1);
            }
            else
            {
                if (lastSpawnedObstaclesInLane.ContainsKey(safeAreaIdx - 1) &&lastSpawnedObstaclesInLane[safeAreaIdx - 1].HasAIPassed)
                    laneIndexes.Add(safeAreaIdx - 1);
            }

            safeAreaIdx = laneIndexes[Random.Range(0, laneIndexes.Count)];
            float distZ = 0, extraOffsetDist = 0;

            for (int i = 0; i < laneIndexes.Count; i++)
            {
                if (i == safeAreaIdx) 
                {
                    continue;
                }

                SetObstaclePositionData(out distZ, out extraOffsetDist, i);
            }

            Transform emptyLane = GetEmptyLane(safeAreaIdx);
            aiController.ChangeLane(emptyLane.position);

            closerEndpointLaneIdx = FindCloserEndpointLaneIdx(safeAreaIdx);
        }

        // if (!lastSpawnedObstaclesInLane.ContainsKey(closerEndpointLaneIdx))
        //     lastSpawnedObstaclesInLane.Add(closerEndpointLaneIdx)

        globalEndPointPos = lastSpawnedObstaclesInLane[closerEndpointLaneIdx].EndPoint.position;
        Debug.Log($"globalEndPointPos: {globalEndPointPos}");
        currentTrackLaneIdx = closerEndpointLaneIdx;
    }

    private void SetObstaclePositionData(out float distZ, out float extraOffsetDist, int i)
    {
        extraOffsetDist = Random.Range(minDistance, maxDistance); // TODO :: tune values
        distZ = safeDistance + extraOffsetDist;

        laneSpawnStartPos = new Vector3(lanes[i].position.x, lanes[i].position.y, aiController.transform.position.z + distZ);

        obstaclesManager.SpawnObstacle(laneSpawnStartPos, out ObstacleBase obstacleBase);

        if (lastSpawnedObstaclesInLane.ContainsKey(i))
            lastSpawnedObstaclesInLane[i] = obstacleBase;
        else
            lastSpawnedObstaclesInLane.Add(i, obstacleBase);
    }

    private Transform GetEmptyLane(int safeAreaIdx)
    {
        var emptyLane = lanes[safeAreaIdx];
        emptyLane.position = new Vector3(emptyLane.position.x, emptyLane.position.y, aiController.transform.position.z + safeDistance);
        return emptyLane;
    }

    private int FindCloserEndpointLaneIdx(int safeAreaIdx)
    {
        int closerEndpoint;
        var n1 = -1;
        var n2 = -1;

        if (safeAreaIdx != lanes.Length && safeAreaIdx != 0)
        {
            n1 = safeAreaIdx - 1;
            n2 = safeAreaIdx + 1;
        }
        else if (safeAreaIdx == 0)
        {
            n1 = safeAreaIdx + 1;
        }
        else
        {
            n1 = safeAreaIdx - 1;
        }

        Vector3 pos1 = Vector3.zero, pos2 = Vector3.zero;

        if (n1 != -1 && lastSpawnedObstaclesInLane.ContainsKey(n1))
        {
            pos1 = lastSpawnedObstaclesInLane[n1].EndPoint.position;
        }

        if (n2 != -1 && lastSpawnedObstaclesInLane.ContainsKey(n2))
        {
            pos2 = lastSpawnedObstaclesInLane[n2].EndPoint.position;
        }

        if (pos1 != Vector3.zero && pos2 != Vector3.zero)
        {
            closerEndpoint = pos1.z < pos2.z ? n1 : n2;
        }
        else// if (pos1 != Vector3.zero)
        {
            closerEndpoint = pos1 != Vector3.zero ? n1 : n2;
        }

        return closerEndpoint;
    }

    public void StartCreatingPathElements()
    {
        obstaclesManager.SetObstaclesType();
        pathTimerLimit = Random.Range(pathTimerMinLimit, pathTimerMaxLimit);
        pathTimer = 0;
        canCreatePath = true;
        Debug.Log($"StartCreatingPathElements");
    }

    private void Update()
    {
        if (canCreatePath)
        {
                CheckIfAICrossedLaneTrainEndPoint();
            // if (pathTimer < pathTimerLimit) // should be configurable
            // {
            //     pathTimer += Time.deltaTime;
            //     CheckIfAICrossedLaneTrainEndPoint();
            // }
            // else
            // {
            //     pathTimer = 0;
            //     canCreatePath = false;
            // }
        }
    }


    private void CheckIfAICrossedLaneTrainEndPoint()
    {
        Debug.Log($"CheckIfAICrossedLaneTrainEndPoint: {aiController.transform.position.z} > {globalEndPointPos.z}");
        if (aiController.transform.position.z > globalEndPointPos.z)
        {
            globalEndPointPos = globalEndPointPosMax;
            if (lastSpawnedObstaclesInLane.ContainsKey(currentTrackLaneIdx))
                lastSpawnedObstaclesInLane[currentTrackLaneIdx].SetAIPassedState(true);
            SpawnObstacles();
        }
    }
}
