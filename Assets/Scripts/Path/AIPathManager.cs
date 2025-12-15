using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using System.Timers;

public class AIPathManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{ 
    [SerializeField] private Transform[] lanes;
    [SerializeField] private int totalLanes = 3;

    [SerializeField] private float pathTimerMinLimit = 6f;
    [SerializeField] private float pathTimerMaxLimit = 10f;

    private float safeDistance = 0f;
    private float timer = 0;
    private float pathTimerLimit = 5f;
    private float extraDelayTime = 2f;

    private bool hasExtraDelay = false;
    private bool canCreatePath;
    private bool isInitialSpawn = true;

    private int currentTrackLaneIdx = -1;
    private int safeAreaIdx = 0;
    
    private List<int> laneIndexes = new List<int>();

    private ObstacleBase lastEncounteredObstacle = null;
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

        if (isInitialSpawn)
        {
            isInitialSpawn = false;

            float distZ = 0, extraOffsetDist = 0;
            safeAreaIdx = Random.Range(0, totalLanes);

            Debug.Log($"Updated safeAreaIdx on initial instance {safeAreaIdx}");

            SetObstaclePositionData();

            closerEndpointLaneIdx = FindCloserEndpointLaneIdx(safeAreaIdx);
        }
        else
        {
            laneIndexes.Clear();
            laneIndexes.Add(safeAreaIdx);
            if (safeAreaIdx != 0 && safeAreaIdx != totalLanes - 1)
            {
                Debug.Log($"safeAreaIdx: {safeAreaIdx}");
                if (lastSpawnedObstaclesInLane.ContainsKey(safeAreaIdx - 1) && lastSpawnedObstaclesInLane[safeAreaIdx - 1].HasAIPassed)
                    laneIndexes.Add(safeAreaIdx - 1);

                if (lastSpawnedObstaclesInLane.ContainsKey(safeAreaIdx + 1) && lastSpawnedObstaclesInLane[safeAreaIdx + 1].HasAIPassed)
                    laneIndexes.Add(safeAreaIdx + 1);
            }
            else if (safeAreaIdx == 0)
            {
                if (lastSpawnedObstaclesInLane.ContainsKey(safeAreaIdx + 1) && lastSpawnedObstaclesInLane[safeAreaIdx + 1].HasAIPassed)
                    laneIndexes.Add(safeAreaIdx + 1);
            }
            else
            {
                if (lastSpawnedObstaclesInLane.ContainsKey(safeAreaIdx - 1) && lastSpawnedObstaclesInLane[safeAreaIdx - 1].HasAIPassed)
                    laneIndexes.Add(safeAreaIdx - 1);
            }

            Debug.Log($":: safeAreaIdx: {safeAreaIdx}");
            Debug.Log($":: laneIndexes: {laneIndexes[0]}, {laneIndexes[1]}");
            safeAreaIdx = laneIndexes[Random.Range(0, laneIndexes.Count)];

            Debug.Log($"Updated safeAreaIdx on second instance {safeAreaIdx}");

            SetObstaclePositionData();

            Transform emptyLane = GetEmptyLane(safeAreaIdx);
            aiController.ChangeLane(emptyLane.position);

            closerEndpointLaneIdx = FindCloserEndpointLaneIdx(safeAreaIdx);
        }

        Debug.Log($"closerEndpointLaneIdx: {closerEndpointLaneIdx}");
        lastEncounteredObstacle = lastSpawnedObstaclesInLane[closerEndpointLaneIdx];
        currentTrackLaneIdx = closerEndpointLaneIdx;
    }

    private void SetObstaclePositionData()
    {
        float extraOffsetDist = 0, distZ = 0;

        for (int i = 0; i < totalLanes; i++)
        {
            if (i == safeAreaIdx)
            {
                continue;
            }

            extraOffsetDist = Random.Range(obstaclesManager.GetObstaclesPathData().extraOffsetMinDist, 
                                            obstaclesManager.GetObstaclesPathData().extraOffsetMaxDist); // TODO :: tune values

            distZ = safeDistance + extraOffsetDist;

            laneSpawnStartPos = new Vector3(lanes[i].position.x, lanes[i].position.y, aiController.transform.position.z + distZ);
            Debug.Log($"LaneSpawnStartPos: {laneSpawnStartPos}");
            obstaclesManager.SpawnObstacle(laneSpawnStartPos, out ObstacleBase obstacleBase);

            if (lastSpawnedObstaclesInLane.ContainsKey(i))
                lastSpawnedObstaclesInLane[i] = obstacleBase;
            else
                lastSpawnedObstaclesInLane.Add(i, obstacleBase);
        }
    }

    private Transform GetEmptyLane(int safeAreaIdx)
    {
        var emptyLane = lanes[safeAreaIdx];
        emptyLane.position = new Vector3(emptyLane.position.x, emptyLane.position.y, aiController.transform.position.z);
        return emptyLane;
    }

    private int FindCloserEndpointLaneIdx(int safeAreaIdx)
    {
        int closerEndpointIdx;
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
            closerEndpointIdx = pos1.z < pos2.z ? n1 : n2;
        }
        else// if (pos1 != Vector3.zero)
        {
            closerEndpointIdx = pos1 != Vector3.zero ? n1 : n2;
        }

        return closerEndpointIdx;
    }

    public void StartCreatingPathElements()
    {
        timer = 0;
        obstaclesManager.SetObstaclesType();

        var obstaclesPathData = obstaclesManager.GetObstaclesPathData();
        safeDistance = obstaclesPathData.safeDistance;
        extraDelayTime = obstaclesPathData.extraDelay;
        hasExtraDelay = extraDelayTime > 0;
        pathTimerLimit = Random.Range(obstaclesPathData.pathTimerMinLimit, obstaclesPathData.pathTimerMaxLimit);

        canCreatePath = true;
        Debug.Log($"StartCreatingPathElements");
    }

    private void Update()
    {
        if (hasExtraDelay)
        {
            if (timer < extraDelayTime)
            {
                timer += Time.deltaTime;
            }    
            else
            {
                timer = 0;
                hasExtraDelay = false;
            }
        }

        if (!hasExtraDelay && canCreatePath)
        {
            // CheckIfAICrossedLaneTrainEndPoint();
            if (timer < pathTimerLimit) // should be configurable
            {
                timer += Time.deltaTime;
                CheckIfAICrossedLaneTrainEndPoint();
            }
            else
            {
                timer = 0;
                canCreatePath = false;
                StartCreatingPathElements();
            }
        }
    }

    private void CheckIfAICrossedLaneTrainEndPoint()
    {
        if (lastEncounteredObstacle)
            Debug.Log($":: CheckIfAICrossedLaneTrainEndPoint :: {aiController.transform.position.z} > {lastEncounteredObstacle.EndPoint.position.z}");

        if (isInitialSpawn || (lastEncounteredObstacle != null && aiController.transform.position.z > lastEncounteredObstacle.EndPoint.position.z))
        {
            lastEncounteredObstacle = null;
            
            if (lastSpawnedObstaclesInLane.ContainsKey(currentTrackLaneIdx))
                lastSpawnedObstaclesInLane[currentTrackLaneIdx].SetAIPassedState(true);

            Debug.Log($"Spawn Obstacles");
            SpawnObstacles();
        }
    }
}
