using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    private PlayerCarController playerCarController;
    private EnvironmentSpawnManager environmentSpawnManager;
    private AIController aiController;

    public void Initialize()
    {
        InterfaceManager.Instance.RegisterInterface<GameManager>(this);
    }

    public void InitializeData()
    {
        environmentSpawnManager = InterfaceManager.Instance?.GetInterfaceInstance<EnvironmentSpawnManager>();
        playerCarController = InterfaceManager.Instance?.GetInterfaceInstance<PlayerCarController>();
        aiController = InterfaceManager.Instance?.GetInterfaceInstance<AIController>();
    }

    public void OnGameOver()
    {
        environmentSpawnManager.UpdateEnvBlockMoveSpeed(0f);
        playerCarController.FollowCamera.SetCamState(false);

        var spawnedMovables = FindObjectsOfType<SpawnableMoverBase>();
        foreach (var spawnedMovable in spawnedMovables)
        {
            spawnedMovable.InitMoveSpeed(0);
        }
        
        aiController.gameObject.SetActive(false);
    }
}
