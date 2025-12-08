using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public enum PoolType
{
    Block = 0,
    Train = 1,
    Obstacle = 2,
    Collectible = 3,
}

public class ObjectPoolManager : MonoBehaviour, IBase, IBootLoader
{
    [SerializeField] private BlockObjectPool[] blockPoolBases;
    [SerializeField] private ObstacleObjectPool[] obstaclePoolBases;

    private ObjectPoolBase<ObjectBase> poolToUse = null;
    // private ItemsObjectPool poolToUse = null;

    private Dictionary<BlockType, BlockObjectPool> blocksPoolBasesDict = new Dictionary<BlockType, BlockObjectPool>();
    private Dictionary<ObstacleType, ObstacleObjectPool> obstaclesPoolBasesDict = new Dictionary<ObstacleType, ObstacleObjectPool>();

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<ObjectPoolManager>(this);
        foreach (var pool in blockPoolBases)
        {
            blocksPoolBasesDict.Add(pool.GetPoolObjectType(), pool);
            pool.InitPoolFirstTime();
        }

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
            case PoolType.Block:
                return (int)Enum.Parse(typeof(BlockType), poolItemTypeInfo);
            case PoolType.Obstacle:
                return (int)Enum.Parse(typeof(ObstacleType), poolItemTypeInfo);
            default: 
                return -1;
        }
    }
    
    public ObjectPoolBase GetUsedPool(int poolIndex, PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.Block:
                var objectType = (BlockType)poolIndex;
                if (blocksPoolBasesDict.ContainsKey(objectType))
                    return blocksPoolBasesDict[objectType];
                else 
                    return null;

            case PoolType.Obstacle:
                var objectType1 = (ObstacleType)poolIndex;
                if (obstaclesPoolBasesDict.ContainsKey(objectType1))
                    return obstaclesPoolBasesDict[objectType1];
                else 
                    return null;
                    
            default:
                return null;
        }
    }
}
