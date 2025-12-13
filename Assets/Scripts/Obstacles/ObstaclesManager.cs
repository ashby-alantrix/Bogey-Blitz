using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrackObstacleType
{
    MovableTrain = 0,
    NonMovableTrain = 1,
    Stopper = 2
}

public enum TrackCollectibleType
{
    Currency = 0,
    Powerup1 = 1,
    Powerup2 = 2,
    Powerup3 = 3
}

public class ObstaclesManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private Transform obstacleEndpoint;

    private ObjectPoolManager objectPoolManager;
    private TrackObstacleType[] obstaclesTypes;

    public Vector3 ObstacleEndpoint => obstacleEndpoint.position;
    public TrackObstacleType CurrentTrackObstacleType
    {
        get;
        private set;    
    }

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<ObstaclesManager>(this);
    }

    public void InitializeData()
    {
        objectPoolManager = InterfaceManager.Instance?.GetInterfaceInstance<ObjectPoolManager>();

        var indexer = 0;
        var obstacleTags = Enum.GetValues(typeof(TrackObstacleType));
        obstaclesTypes = new TrackObstacleType[obstacleTags.Length];

        foreach (var obstacleTag in obstacleTags)
        {
            obstaclesTypes[indexer++] = (TrackObstacleType)obstacleTag;
        }  
    }

    public void SpawnObstacle(Vector3 laneSpawnStartPos, out ObstacleBase obstacleBase)
    {
        var poolInstance = objectPoolManager.GetObjectFromPool<ObstacleBase>($"{CurrentTrackObstacleType}", GetPoolType());

        Debug.Log($"");

        poolInstance.transform.position = laneSpawnStartPos;
        poolInstance.gameObject.SetActive(true);
        obstacleBase = poolInstance;
    }

    public void SendObjectToPool(ObstacleBase obstacleBase)
    {
        objectPoolManager.PassObjectToPool($"{obstacleBase.ObjectType}", GetPoolType(), obstacleBase);
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
                return PoolType.Movable;
        }
    }

    public void SetObstaclesType()
    {
        CurrentTrackObstacleType = obstaclesTypes[UnityEngine.Random.Range(0, obstaclesTypes.Length)];
    }
}
