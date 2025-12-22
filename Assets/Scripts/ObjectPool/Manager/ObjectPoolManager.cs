using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public enum PoolType
{
    Movable = 0,
    NonMovable = 1,
    Obstacle = 2,
    Currency = 3,
    Powerup = 4,
    MAX = 5,
}

public class ObjectPoolManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private ObstacleObjectPool[] obstaclePoolBases;
    [SerializeField] private CollectibleObjectPool[] collectiblePoolBases;

    private Dictionary<TrackObstacleType, ObjectPoolBase> obstaclesPoolBasesDict = new Dictionary<TrackObstacleType, ObjectPoolBase>();
    private Dictionary<TrackCollectibleType, ObjectPoolBase> collectiblesPoolBasesDict = new Dictionary<TrackCollectibleType, ObjectPoolBase>();

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<ObjectPoolManager>(this);
    }

    public void InitializeData()
    {
        foreach (var pool in obstaclePoolBases)
        {
            obstaclesPoolBasesDict.Add(pool.GetPoolObjectType(), pool);
            pool.InitPoolFirstTime();
        }

        foreach (var pool in collectiblePoolBases)
        {
            collectiblesPoolBasesDict.Add(pool.GetPoolObjectType(), pool);
            pool.InitPoolFirstTime();
        }
    }
 
    public T GetObjectFromPool<T>(string poolItemTypeInfo, PoolType poolType) where T : ObjectBase //ItemType poolItemType) where T : ObjectBase
    {
        T objectBase = null;
        ObjectPoolBase poolToUse = GetUsedPool(GetPoolInfoBasedOnType(poolItemTypeInfo, poolType), poolType);

        if (poolToUse != null)
        {
            if (poolToUse.IsEmpty())
            {
                Debug.Log($"Creating new pooled item");
                // Debug.Log($"Object Pool: CreateNewPooledItem");
                objectBase = (T)poolToUse.CreateNewPooledItem();
            }
            else
            {
                // Debug.Log($"Object Pool: Use existing item");
                objectBase = (T)poolToUse.Dequeue();
            }
        }

        return objectBase;
    }

    public void PassObjectToPool<T>(string poolItemTypeInfo, PoolType poolType, T objectBase) where T : ObjectBase
    {
        ObjectPoolBase poolToUse = GetUsedPool(GetPoolInfoBasedOnType(poolItemTypeInfo, poolType), poolType);

        // Debug.Log($"Object Pool: PassObjectToPool: PoolToUse {poolToUse != null}");
        if (poolToUse != null)
        {
            poolToUse.Enqueue(objectBase);
        }
    }

    private int GetPoolInfoBasedOnType(string poolItemTypeInfo, PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.Movable:
            case PoolType.NonMovable:
            case PoolType.Obstacle:
                return (int)Enum.Parse(typeof(TrackObstacleType), poolItemTypeInfo);
            case PoolType.Currency:
                return (int)Enum.Parse(typeof(TrackCollectibleType), poolItemTypeInfo);
            default: 
                return -1;
        }
    }
    
    public ObjectPoolBase GetUsedPool(int poolIndex, PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.Movable:
            case PoolType.NonMovable:
            case PoolType.Obstacle:
                var trackObstacleType = (TrackObstacleType)poolIndex;
                if (obstaclesPoolBasesDict.ContainsKey(trackObstacleType))
                    return obstaclesPoolBasesDict[trackObstacleType];
                else 
                    return null;
            case PoolType.Currency:
                var trackCollectibleType = (TrackCollectibleType)poolIndex;
                if (collectiblesPoolBasesDict.ContainsKey(trackCollectibleType))
                    return collectiblesPoolBasesDict[trackCollectibleType];
                else 
                    return null;
            default:
                return null;
        }
    }
}
