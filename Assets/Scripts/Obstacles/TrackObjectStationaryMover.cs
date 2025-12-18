using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackObjectStationaryMover : SpawnableMoverBase
{
    private ObstaclesManager obstaclesManager;

    private void Update()
    {
        if (!worldSpawnManager)
        {
            worldSpawnManager = InterfaceManager.Instance?.GetInterfaceInstance<WorldSpawnManager>();
        }
        
        if (worldSpawnManager)
        {
            moveSpeed = worldSpawnManager.EnvironmentMoveSpeed;
            base.Update();
        }
    }
}
