using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTrainMover : SpawnableMoverBase
{
    private ObstaclesManager obstaclesManager;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        SetObstaclesManager();
        obstaclesManager?.GameManager?.SoundManager?.RegisterToMultiSources(audioSource);
    }

    private void OnDisable()
    {
        SetObstaclesManager();
        obstaclesManager?.GameManager?.SoundManager?.UnregisterFromMultiSources();
    }

    protected override void Update()
    {
        if (!worldSpawnManager || !obstaclesManager)
        {
            worldSpawnManager = InterfaceManager.Instance?.GetInterfaceInstance<WorldSpawnManager>();
            SetObstaclesManager();
        }
        
        if (worldSpawnManager && obstaclesManager)
        {
            moveSpeed = obstaclesManager.MovableTrainSpeed;
            Debug.Log($"## obstaclesManager.MovableTrainSpeed: {obstaclesManager.MovableTrainSpeed}");
            base.Update();
        }
    }

    private void SetObstaclesManager()
    {
        obstaclesManager = InterfaceManager.Instance?.GetInterfaceInstance<ObstaclesManager>();
    }
    
}
