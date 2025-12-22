using System;
using UnityEngine;

public enum GameState
{
    GameStart,
    GameInProgress,
    GamePaused,
    GameRestart,
    GameOver,
}

public class GameManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    private PlayerCarController playerCarController;
    private WorldSpawnManager worldSpawnManager;
    private AIController aiController;
    private ObstaclesManager obstaclesManager;
    private CollectiblesManager collectiblesManager;
    private InGameUIManager inGameUIManager;

    private GameState currentGameState;
    // {
    //     get;
    //     private set;
    // }

    public bool IsGameInProgress => currentGameState == GameState.GameInProgress;

    public void Initialize()
    {
        InterfaceManager.Instance.RegisterInterface<GameManager>(this);
    }

    public void InitializeData()
    {
        worldSpawnManager = InterfaceManager.Instance?.GetInterfaceInstance<WorldSpawnManager>();
        playerCarController = InterfaceManager.Instance?.GetInterfaceInstance<PlayerCarController>();
        aiController = InterfaceManager.Instance?.GetInterfaceInstance<AIController>();
        inGameUIManager = InterfaceManager.Instance?.GetInterfaceInstance<InGameUIManager>();

        obstaclesManager = InterfaceManager.Instance?.GetInterfaceInstance<ObstaclesManager>();
        collectiblesManager = InterfaceManager.Instance?.GetInterfaceInstance<CollectiblesManager>();
    }

    public void OnGameStateChange(GameState state)
    {
        currentGameState = state;
        switch (currentGameState)
        {
            case GameState.GameStart:
                OnGameStart();
            break;
            case GameState.GameInProgress:
                OnGameProgress();
            break;
            case GameState.GameRestart:
                OnGameRestart();
            break;
            case GameState.GamePaused:
                playerCarController.ResetEnvironmentBaseSpeed();
                inGameUIManager.PopupManager.ShowPopup(PopupType.Pause);
            break;
            case GameState.GameOver:
                OnGameOver();
            break;
        }
    }

    private void OnGameStart()
    {
        playerCarController.PlayerCollisionHandler.SetupNewCrashModel();
        playerCarController.PlayerCollisionHandler.ActivateCarModel(true);
        playerCarController.FollowCamera.SetCamState(true);
        playerCarController.SetBaseMovementSpeed();
        
        aiController.gameObject.SetActive(true);
        aiController.AIPathManager.InitializeTimerSystem();
        aiController.AIPathManager.StartCreatingObstacleElements();
        
        OnGameStateChange(GameState.GameInProgress);
    }

    private void OnGameProgress()
    {
        inGameUIManager.ScreenManager.ShowScreen(ScreenType.InGameHUDScreen);
    }

    private void OnGameOver()
    {
        Debug.Log($":: OnGameOver");
        worldSpawnManager.SetEnvironmentMoveSpeed(0f);
        playerCarController.FollowCamera.SetCamState(false);
        ActivatePlayer(false);

        aiController.gameObject.SetActive(false);

        inGameUIManager.PopupManager.ShowPopup(PopupType.GameOver);
    }

    public void ActivatePlayer(bool state)
    {
        playerCarController.enabled = state;
    }

    private void OnGameRestart()
    {
        /// <summary>
        /// 
        /// -> send all obstacles to the pool
        /// -> send all collectibles to pool
        /// 
        /// -> reset the distance
        /// -> stop ai logic
        /// 
        /// -> reset the envBaseSpeed
        /// -> reset the car position
        /// -> start path-finding
        /// 
        /// </summary>
        
        obstaclesManager.SendAllObjectsToPool();
        collectiblesManager.SendAllObjectToPool();

        playerCarController.ResetData();

        OnGameStateChange(GameState.GameStart);
    }
}
