using System;
using UnityEngine;

public enum GameState
{
    GameMenu,
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
    public SoundManager SoundManager
    {
        get;
        private set;
    }
    public InGameUIManager InGameUIManager
    {
        get;
        private set;
    }

    private GameState currentGameState;
    // {
    //     get;
    //     private set;
    // }

    public bool IsGameInProgress => currentGameState == GameState.GameInProgress;

    public Action OnGameBackInProgress;

    public void Initialize()
    {
        InterfaceManager.Instance.RegisterInterface<GameManager>(this);
    }

    public void InitializeData()
    {
        worldSpawnManager = InterfaceManager.Instance?.GetInterfaceInstance<WorldSpawnManager>();
        playerCarController = InterfaceManager.Instance?.GetInterfaceInstance<PlayerCarController>();
        aiController = InterfaceManager.Instance?.GetInterfaceInstance<AIController>();
        InGameUIManager = InterfaceManager.Instance?.GetInterfaceInstance<InGameUIManager>();
        SoundManager = InterfaceManager.Instance?.GetInterfaceInstance<SoundManager>();

        obstaclesManager = InterfaceManager.Instance?.GetInterfaceInstance<ObstaclesManager>();
        collectiblesManager = InterfaceManager.Instance?.GetInterfaceInstance<CollectiblesManager>();

        Debug.Log($"Initializing playerCarController: {playerCarController}");

        OnGameStateChange(GameState.GameMenu);
    }

    public void OnGameStateChange(GameState state)
    {
        currentGameState = state;
        switch (currentGameState)
        {
            case GameState.GameMenu:
                OnGameMenuOpened();
                break;
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
                OnGamePaused();
                break;
            case GameState.GameOver:
                OnGameOver();
            break;
        }
    }

    private void OnGamePaused()
    {
        SoundManager.PlayMusic(SingleInstAudioSourceType.BG, false);
        SoundManager.PlayMusic(SingleInstAudioSourceType.CarAccel, false);

        playerCarController.ResetEnvironmentBaseSpeed();
        InGameUIManager.PopupManager.ShowPopup(PopupType.Pause);
    }

    private void OnGameMenuOpened()
    {
        SoundManager.PlayMusic(SingleInstAudioSourceType.BG, false);
        SoundManager.PlayMusic(SingleInstAudioSourceType.CarAccel, false);
        
        ResetGameplayData();
        ResetTrackElements();
        ResetUIPanels();
        InGameUIManager.ScreenManager.ShowScreen(ScreenType.MainMenu);
    }

    private void ResetUIPanels()
    {
        InGameUIManager.ScreenManager.HideAllScreens();
        InGameUIManager.PopupManager.HideAllPopups();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
            OnGameStateChange(GameState.GamePaused);
#endif
    }

    private void OnGameStart()
    {
        Debug.Log($"Initializing playerCarController :: gamestart : {playerCarController}");

        playerCarController.PlayerCollisionHandler.SetupNewCrashModel();
        playerCarController.PlayerCollisionHandler.ActivateCarModel(true);
        playerCarController.FollowCamera.SetCamState(true);
        playerCarController.SetBaseMovementSpeed();
        ActivatePlayer(true);
        
        aiController.gameObject.SetActive(true);
        aiController.AIPathManager.InitializeTimerSystem();
        aiController.AIPathManager.StartCreatingObstacleElements();
        
        OnGameStateChange(GameState.GameInProgress);
    }

    private void OnGameProgress()
    {
        SoundManager.PlayMusic(SingleInstAudioSourceType.BG, true);
        SoundManager.PlayMusic(SingleInstAudioSourceType.CarAccel, true);

        Debug.Log($"OnGameProgress :: ShowScreen ScreenType.InGameHUDScreen");
        InGameUIManager.ScreenManager.ShowScreen(ScreenType.InGameHUDScreen);
        OnGameBackInProgress?.Invoke();
        Invoke(nameof(DeallocateProgressEvent), 0.25f);
    }

    private void DeallocateProgressEvent()
    {
        OnGameBackInProgress = null;
    }

    private void OnGameOver()
    {
        Debug.Log($":: OnGameOver");
        
        SoundManager.PlayPrimaryGameSoundClip(SoundType.CarCrash);
        SoundManager.PlayMusic(SingleInstAudioSourceType.BG, false);
        SoundManager.PlayMusic(SingleInstAudioSourceType.CarAccel, false);

        worldSpawnManager.SetEnvironmentMoveSpeed(0f);
        ResetGameplayData();

        InGameUIManager.PopupManager.ShowPopup(PopupType.GameOver);
    }

    private void ResetGameplayData()
    {
        playerCarController.FollowCamera.SetCamState(false);
        ActivatePlayer(false);

        aiController.gameObject.SetActive(false);
    }

    public void ActivatePlayer(bool state)
    {
        playerCarController.enabled = state;
    }

    private void OnGameRestart()
    {
        ResetTrackElements();
        OnGameStateChange(GameState.GameStart);
    }

    private void ResetTrackElements()
    {
        obstaclesManager.SendAllObjectsToPool();
        collectiblesManager.SendAllObjectToPool();

        playerCarController.ResetData();
    }
}
