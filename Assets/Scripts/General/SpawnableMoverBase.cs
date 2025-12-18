using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableMoverBase : MonoBehaviour
{
    protected float moveSpeed;

    protected WorldSpawnManager worldSpawnManager;

    private ObstacleBase obstacleBase;
    private ObstaclesManager obstaclesManager;
    private WorldSpawnManager worldSpawnManager;

    private void Awake()
    {
        obstacleBase = GetComponent<ObstacleBase>();
    }

    public void InitMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    protected void Update()
    {
        if (!worldSpawnManager || !obstaclesManager)
        {
            worldSpawnManager = InterfaceManager.Instance?.GetInterfaceInstance<WorldSpawnManager>();
            obstaclesManager = InterfaceManager.Instance?.GetInterfaceInstance<ObstaclesManager>();
        }
        else 
        {
            moveSpeed = obstacleBase.ObstacleType != TrackObstacleType.MovableTrain ? worldSpawnManager.EnvironmentMoveSpeed : obstaclesManager.MovableTrainSpeed;
            transform.position += transform.forward * Time.deltaTime * moveSpeed;
        }
    }
}
