using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackObjectStationaryMover : SpawnableMoverBase
{
    protected override void Update()
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
