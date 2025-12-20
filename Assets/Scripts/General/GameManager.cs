using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    private PlayerCarController playerCarController;
    private WorldSpawnManager worldSpawnManager;
    private AIController aiController;

    public void Initialize()
    {
        InterfaceManager.Instance.RegisterInterface<GameManager>(this);
    }

    public void InitializeData()
    {
        worldSpawnManager = InterfaceManager.Instance?.GetInterfaceInstance<WorldSpawnManager>();
        playerCarController = InterfaceManager.Instance?.GetInterfaceInstance<PlayerCarController>();
        aiController = InterfaceManager.Instance?.GetInterfaceInstance<AIController>();
    }

    public void OnGameOver()
    {
        Debug.Log($":: OnGameOver");
        worldSpawnManager.SetEnvironmentMoveSpeed(0f);
        playerCarController.FollowCamera.SetCamState(false);
        ActivatePlayer(false);

        aiController.gameObject.SetActive(false);
    }

    public void ActivatePlayer(bool state)
    {
        playerCarController.enabled = state;
    }
}
