using UnityEngine;

public enum GameState
{
    GameStart,
    GameInProgress,
    GameOver,
}

public class GameManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    private PlayerCarController playerCarController;
    private WorldSpawnManager worldSpawnManager;
    private AIController aiController;

    private GameState currentGameState;
    // {
    //     get;
    //     private set;
    // }

    public bool IsGameInProgress => currentGameState == GameState.GameStart;

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

    public void OnGameStateChange(GameState state)
    {
        currentGameState = state;
        switch (currentGameState)
        {
            case GameState.GameStart:
                OnGameStart();
            break;
            case GameState.GameOver:
                OnGameOver();
            break;
        }
    }

    private void OnGameStart()
    {
        playerCarController.SetEnvironmentBaseSpeed();
        // aiController.AIPathManager.StartCreatingObstacleElements();
    }

    private void OnGameOver()
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
