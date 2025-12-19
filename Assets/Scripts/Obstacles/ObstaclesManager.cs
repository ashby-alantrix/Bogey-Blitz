using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public enum TrackObstacleType
{
    Stopper = 0,
    NonMovableTrain = 1,
    MovableTrain = 2,
    MAX = 3,
}

public class ObstaclesManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private ObstaclesPathSO obstaclesPathSO;
    [SerializeField] private Transform obstacleEndpoint;
    [SerializeField] private float movableTrainSpeed;

    [SerializeField] private float movableTrainMinSpeed = 30f;
    [SerializeField] private float movableTrainMaxSpeed = 45f;

    private ObjectPoolManager objectPoolManager;
    private WorldSpawnManager worldSpawnManager;
    private PlayerCarController playerCarController;

    public ObstaclesPathSO ObstaclesPathSO => obstaclesPathSO;
    public Vector3 ObstacleEndpoint => obstacleEndpoint.position;
    public TrackObstacleType CurrentTrackObstacleType
    {
        get;
        private set;    
    }

    public float MovableTrainSpeed => worldSpawnManager.GetResultBasedOnDifficultyProgressiveFormula(startVal: movableTrainMinSpeed, endVal: movableTrainMaxSpeed, fraction: playerCarController.CurrentEvaluatedSpeed01);

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<ObstaclesManager>(this);
    }

    public void InitializeData()
    {
        objectPoolManager = InterfaceManager.Instance?.GetInterfaceInstance<ObjectPoolManager>();
        worldSpawnManager = InterfaceManager.Instance?.GetInterfaceInstance<WorldSpawnManager>();
        playerCarController = InterfaceManager.Instance?.GetInterfaceInstance<PlayerCarController>();
    }

    public void SpawnObstacle(Vector3 laneSpawnStartPos, out ObstacleBase obstacleBase)
    {
        ObstacleBase poolInstance = objectPoolManager.GetObjectFromPool<ObstacleBase>($"{CurrentTrackObstacleType}", GetPoolType());

        poolInstance.transform.position = laneSpawnStartPos;
        poolInstance.gameObject.SetActive(true);

        obstacleBase = poolInstance;
    }

    public void SendObjectToPool(ObstacleBase obstacleBase)
    {
        objectPoolManager.PassObjectToPool($"{obstacleBase.ObstacleType}", GetPoolType(), obstacleBase);
    }

    public PoolType GetPoolType()
    {
        switch (CurrentTrackObstacleType)
        {
            case TrackObstacleType.MovableTrain:
                return PoolType.Movable;
            case TrackObstacleType.NonMovableTrain:
                return PoolType.NonMovable;
            case TrackObstacleType.Stopper:
                return PoolType.Obstacle;
            default:
                return PoolType.MAX;
        }
    }

    public void SetObstaclesType(bool isInitialSpawn)
    {
        CurrentTrackObstacleType = (TrackObstacleType)(isInitialSpawn ? UnityEngine.Random.Range(0, (int)TrackObstacleType.MovableTrain) 
                                                                      : UnityEngine.Random.Range(0, (int)TrackObstacleType.MAX));
    }
}
