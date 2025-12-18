using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    private PlayerCarController playerCarController;
    private WorldSpawnManager environmentSpawnManager;
    private AIController aiController;

    public void Initialize()
    {
        InterfaceManager.Instance.RegisterInterface<GameManager>(this);
    }

    public void InitializeData()
    {
        environmentSpawnManager = InterfaceManager.Instance?.GetInterfaceInstance<WorldSpawnManager>();
        playerCarController = InterfaceManager.Instance?.GetInterfaceInstance<PlayerCarController>();
        aiController = InterfaceManager.Instance?.GetInterfaceInstance<AIController>();
    }

    public void OnGameOver()
    {
        environmentSpawnManager.UpdateEnvBlockMoveSpeed(0f);
        playerCarController.FollowCamera.SetCamState(false);
        
        aiController.gameObject.SetActive(false);
    }
}
