using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTrainMover : SpawnableMoverBase
{
    private ObstaclesManager obstaclesManager;

    protected override void Update()
    {
        if (!worldSpawnManager || !obstaclesManager)
        {
            worldSpawnManager = InterfaceManager.Instance?.GetInterfaceInstance<WorldSpawnManager>();
            obstaclesManager = InterfaceManager.Instance?.GetInterfaceInstance<ObstaclesManager>();
        }
        
        if (worldSpawnManager && obstaclesManager)
        {
            moveSpeed = obstaclesManager.MovableTrainSpeed;
            base.Update();
        }
    }
}
