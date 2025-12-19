using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleMoverBase : SpawnableMoverBase
{
    private CollectibleBase obstacleBase;
    private ObstaclesManager obstaclesManager;

    private void Awake()
    {
        obstacleBase = GetComponent<CollectibleBase>();
    }

    private void Update()
    {
        if (!worldSpawnManager || !obstaclesManager)
        {
            worldSpawnManager = InterfaceManager.Instance?.GetInterfaceInstance<WorldSpawnManager>();
            obstaclesManager = InterfaceManager.Instance?.GetInterfaceInstance<ObstaclesManager>();
        }
        
        if (worldSpawnManager && obstaclesManager)
        {
            moveSpeed = worldSpawnManager.EnvironmentMoveSpeed;
            base.Update();
        }
    }
}
