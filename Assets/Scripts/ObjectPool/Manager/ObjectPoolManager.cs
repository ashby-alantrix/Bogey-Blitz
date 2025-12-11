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
    Collectible = 3,
}

public class ObjectPoolManager : MonoBehaviour, IBase, IBootLoader
{
    [SerializeField] private ObstacleObjectPool[] obstaclePoolBases;

    private Dictionary<TrackObstacleType, ObjectPoolBase> obstaclesPoolBasesDict = new Dictionary<TrackObstacleType, ObjectPoolBase>();
    private Dictionary<TrackCollectibleType, ObjectPoolBase> collectiblesPoolBasesDict = new Dictionary<TrackCollectibleType, ObjectPoolBase>();

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<ObjectPoolManager>(this);
        
        foreach (var pool in obstaclePoolBases)
        {
            obstaclesPoolBasesDict.Add(pool.GetPoolObjectType(), pool);
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
            case PoolType.Collectible:
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
            case PoolType.Collectible:
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
