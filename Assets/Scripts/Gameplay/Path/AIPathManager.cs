using System.Linq.Expressions;
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

    [SerializeField] private float pathTimerMinLimit = 6f;
    [SerializeField] private float pathTimerMaxLimit = 10f;

    // private float safeDistance = 0f;
    private float timer = 0;
    private float pathTimerLimit = 5f;
    // private float extraDelayTime = 2f;

    private bool isInitialSpawn = true;

    private int currentTrackLaneIdx = -1;
    private int safeAreaIdx = 0;
    private float nonMovableDifficultyVal = 0;
    private float movableDifficultyVal = 0;

    private Vector3 globalEndPointPosMax = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
    private List<int> laneIndexes = new List<int>();
    private Vector3 laneSpawnStartPos;

    private TimerSystem timerSystem;
    private PlayerCarController playerCarController;
    private AIController aiController;
    private WorldSpawnManager worldSpawnManager;
    private ObstaclesManager obstaclesManager;
    private CollectiblesManager collectiblesManager;
    private DifficultyEvaluator difficultyEvaluator;
    private GameManager gameManager;
    private ObstacleBase lastEncounteredObstacle = null;
    
    private AnimationCurve nonMoveableObstaclesDiffCurve;
    private AnimationCurve moveableTrainDiffCurve;

    private Dictionary<int, ObstacleBase> lastSpawnedObstaclesInLane = new Dictionary<int, ObstacleBase>(); // convert to a list if required

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<AIPathManager>(this);
    }

    public void InitializeData()
    {
        playerCarController = InterfaceManager.Instance?.GetInterfaceInstance<PlayerCarController>();
        aiController = InterfaceManager.Instance?.GetInterfaceInstance<AIController>();
        obstaclesManager = InterfaceManager.Instance?.GetInterfaceInstance<ObstaclesManager>();
        collectiblesManager = InterfaceManager.Instance?.GetInterfaceInstance<CollectiblesManager>();
        worldSpawnManager = InterfaceManager.Instance?.GetInterfaceInstance<WorldSpawnManager>();
        difficultyEvaluator = InterfaceManager.Instance?.GetInterfaceInstance<DifficultyEvaluator>();
        gameManager = InterfaceManager.Instance?.GetInterfaceInstance<GameManager>();

        InitDifficultyCurves();
        InitializeTimerSystem();
    }

    private void InitDifficultyCurves()
    {
        Debug.Log($"### InitDifficultyCurves");
        moveableTrainDiffCurve = difficultyEvaluator.DifficultyCurveSO.GetDifficultyCurve(DifficultyCurveType.MovableTrainSpeed);
        nonMoveableObstaclesDiffCurve = difficultyEvaluator.DifficultyCurveSO.GetDifficultyCurve(DifficultyCurveType.NonMovableObstacleSpeed);
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
            // Debug.Log($":: laneIndexes: {laneIndexes[0]}, {laneIndexes[1]}");
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

    private ObstaclesPathData obstaclesPathData = null;

    public void StartCreatingObstacleElements()
    {
        timer = 0;
        InitObstaclesPathData();
        
        Debug.Log($"### obstaclesManager: {obstaclesManager.CurrentTrackObstacleType}");
        Debug.Log($"### movableDifficultyCurve: {moveableTrainDiffCurve.Evaluate(playerCarController.CurrentCoveredDistance01)}");
        Debug.Log($"### nonMovableObstaclesDifficultyCurve: {nonMoveableObstaclesDiffCurve.Evaluate(playerCarController.CurrentCoveredDistance01)}");

        var difficultyVal = obstaclesManager.CurrentTrackObstacleType == TrackObstacleType.MovableTrain 
                                                    ? moveableTrainDiffCurve.Evaluate(playerCarController.CurrentCoveredDistance01) 
                                                    : nonMoveableObstaclesDiffCurve.Evaluate(playerCarController.CurrentCoveredDistance01);

        var startBoundPathTimerMax = obstaclesManager.ObstaclesPathSO.GetStartBoundObstaclesPathData(obstaclesManager.CurrentTrackObstacleType).pathTimerMaxLimit;
        var endBoundPathTimerMax = obstaclesManager.ObstaclesPathSO.GetEndBoundObstaclesPathData(obstaclesManager.CurrentTrackObstacleType).pathTimerMaxLimit;

        obstaclesPathData.pathTimerMaxLimit = worldSpawnManager.GetResultBasedOnDifficultyProgressiveFormula(startVal: startBoundPathTimerMax, endVal: endBoundPathTimerMax, fraction: difficultyVal);

        var startBoundSafeDistance = obstaclesManager.ObstaclesPathSO.GetStartBoundObstaclesPathData(obstaclesManager.CurrentTrackObstacleType).safeDistance;
        var endBoundSafeDistance = obstaclesManager.ObstaclesPathSO.GetEndBoundObstaclesPathData(obstaclesManager.CurrentTrackObstacleType).safeDistance;

        obstaclesPathData.safeDistance = worldSpawnManager.GetResultBasedOnDifficultyProgressiveFormula(startVal: startBoundSafeDistance, endVal: endBoundSafeDistance, fraction: difficultyVal);

        Debug.Log($"## obstaclesPathData.safeDistance: {obstaclesPathData.safeDistance}");
        Debug.Log($"## obstaclesPathData.pathTimerMaxLimit: {obstaclesPathData.pathTimerMaxLimit}");

        pathTimerLimit = Random.Range(obstaclesPathData.pathTimerMinLimit, obstaclesPathData.pathTimerMaxLimit);

        if (obstaclesPathData.extraDelay > 0)
        {
            InitializePathTimerWithExtraDelay();
        }
        else
        {
            InitializePathTimer();
        }

        Debug.Log($"StartCreatingPathElements");
    }

    private void InitObstaclesPathData()
    {
        obstaclesManager.SetObstaclesType(isInitialSpawn);
        var newData = obstaclesManager.ObstaclesPathSO.GetStartBoundObstaclesPathData(obstaclesManager.CurrentTrackObstacleType);

        if (obstaclesPathData == null)
        {
            obstaclesPathData = new ObstaclesPathData();
        }

        obstaclesPathData.trackObstacleType = newData.trackObstacleType;
        obstaclesPathData.extraDelay = newData.extraDelay;
        obstaclesPathData.pathTimerMinLimit = newData.pathTimerMinLimit;
        obstaclesPathData.pathTimerMaxLimit = newData.pathTimerMaxLimit;
        obstaclesPathData.safeDistance = newData.safeDistance;
        obstaclesPathData.extraOffsetMinDist = newData.extraOffsetMinDist;
        obstaclesPathData.extraOffsetMaxDist = newData.extraOffsetMaxDist;
    }

    public void CreateCollectibleElements()
    {
        // TODO :: create two different positions

        int index1 = Random.Range(0, lanes.Length);
        int index2 = Random.Range(0, lanes.Length);
        Vector3 lanePos1 = new Vector3(lanes[index1].position.x, 0.5f, aiController.transform.position.z);
        collectiblesManager.SpawnCollectible(lanePos1);
        if (index1 != index2)
        {
            Vector3 lanePos2 = new Vector3(lanes[index2].position.x, 0.5f, aiController.transform.position.z);
            collectiblesManager.SpawnCollectible(lanePos2);
        }
    }

    public void InitializeTimerSystem()
    {
        isInitialSpawn = true;
        timerSystem = new TimerSystem();
    }

    private void InitializePathTimer()
    {
        timerSystem.Init(pathTimerLimit,
        onComplete: () =>
        {
            StartCreatingObstacleElements();
        },
        inProgress: () =>
        {
            CheckIfAICrossedLaneTrainEndPoint();
        });
    }

    private void InitializePathTimerWithExtraDelay()
    {
        timerSystem.Init(obstaclesPathData.extraDelay,
        onComplete: () =>
        {
            timerSystem.Init(pathTimerLimit,
            onComplete: () =>
            {
                StartCreatingObstacleElements();
            },
            inProgress: () =>
            {
                CheckIfAICrossedLaneTrainEndPoint();
            });
        });
    }

    private void Update()
    {
        if (!gameManager || !gameManager.IsGameInProgress) return;

        timerSystem?.UpdateTimer(Time.deltaTime);
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

    private void SetObstaclePositionData()
    {
        float extraOffsetDist = 0, distZ = 0;

        for (int i = 0; i < totalLanes; i++)
        {
            if (i == safeAreaIdx)
            {
                continue;
            }

            extraOffsetDist = Random.Range(obstaclesManager.ObstaclesPathSO.GetStartBoundObstaclesPathData(obstaclesManager.CurrentTrackObstacleType).extraOffsetMinDist, 
                                            obstaclesManager.ObstaclesPathSO.GetStartBoundObstaclesPathData(obstaclesManager.CurrentTrackObstacleType).extraOffsetMaxDist); // TODO :: tune values

            distZ = obstaclesPathData.safeDistance + extraOffsetDist;

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
}
