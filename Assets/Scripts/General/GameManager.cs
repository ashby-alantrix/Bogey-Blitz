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
    private CurrencyManager currencyManager;
    private UserDataBehaviour userDataBehaviour;

    private InGameData inGameData;

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
        currencyManager = InterfaceManager.Instance?.GetInterfaceInstance<CurrencyManager>();
        userDataBehaviour = InterfaceManager.Instance?.GetInterfaceInstance<UserDataBehaviour>();

        obstaclesManager = InterfaceManager.Instance?.GetInterfaceInstance<ObstaclesManager>();
        SetCollectiblesManager();

        Debug.Log($"Initializing playerCarController: {playerCarController}");

        inGameData = userDataBehaviour.GetInGameData();

        OnGameStateChange(GameState.GameMenu);
    }

    private void SetCollectiblesManager()
    {
        collectiblesManager = InterfaceManager.Instance?.GetInterfaceInstance<CollectiblesManager>();
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
        SoundManager.PlayBGAudio(false);
        SoundManager.PlayCarAudio(false);
        SoundManager.SetMultiSourcesState(false);

        playerCarController.ResetEnvironmentBaseSpeed();
        InGameUIManager.PopupManager.ShowPopup(PopupType.Pause);
    }

    private void OnGameMenuOpened()
    {
        if (collectiblesManager.CollectibleCoins > 0)
            currencyManager.AddCurrency(collectiblesManager.CollectibleCoins);

        SoundManager.PlayBGAudio(false);
        SoundManager.PlayCarAudio(false);
        SoundManager.SetMultiSourcesState(false);
        
        ResetGameplayData();
        // ResetTrackElements();
        ResetUIPanels();
        OnNewSessionOrPlaySessionComplete();
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
        Debug.Log($"userDataBehaviour.HasSeenInstruction() : {userDataBehaviour.HasSeenInstruction()}");

        if (!userDataBehaviour.HasSeenInstruction())
        {
            userDataBehaviour.ToggleHasSeenInstructionState(true);
            InGameUIManager.ShowInstructionPopup();
            return;
        }

        playerCarController.PlayerCollisionHandler.SetupNewCrashModel();
        playerCarController.PlayerCollisionHandler.ActivateCarModel(true);
        playerCarController.FollowCamera.SetCamState(true);
        playerCarController.SetBaseMovementSpeed();
        ActivatePlayer(true);
        
        aiController.gameObject.SetActive(true);
        aiController.AIPathManager.InitializeTimerSystem();
        // aiController.AIPathManager.StartCreatingObstacleElements();
        
        OnGameStateChange(GameState.GameInProgress);
    }

    private void OnGameProgress()
    {
        SoundManager.PlayBGAudio(true);
        SoundManager.PlayCarAudio(true);
        SoundManager.SetMultiSourcesState(true);

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
        SoundManager.PlayBGAudio(false);
        SoundManager.PlayCarAudio(false);
        SoundManager.SetMultiSourcesState(false);

        worldSpawnManager.SetEnvironmentMoveSpeed(0f);
        ResetGameplayData();

        if (playerCarController.CurrentCoveredDistance > userDataBehaviour.GetInGameData().highestDistanceCovered)
        {
            inGameData.highestDistanceCovered = Mathf.FloorToInt(playerCarController.CurrentCoveredDistance);
            userDataBehaviour.SaveInGameData(inGameData);
            InGameUIManager.PopupManager.GetPopup<HighScorePopup>(PopupType.HighScore).InitInfo(inGameData.highestDistanceCovered, collectiblesManager.CollectibleCoins);
            InGameUIManager.PopupManager.ShowPopup(PopupType.HighScore);
        }
        else 
        {
            InGameUIManager.PopupManager.ShowPopup(PopupType.GameOver);
        }
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
        Debug.Log($"#### OnGameRestart Distance Covered: {playerCarController.CurrentCoveredDistance}");
        Debug.Log($"#### OnGameRestart after resetting Distance Covered: {playerCarController.CurrentCoveredDistance}");

        currencyManager.AddCurrency(collectiblesManager.CollectibleCoins);
        OnNewSessionOrPlaySessionComplete();
        OnGameStateChange(GameState.GameStart);
    }

    private void OnNewSessionOrPlaySessionComplete()
    {
        ResetTrackElements();
        InGameUIManager.ResetUIData();
        playerCarController.OnDataReset();

        SetCollectiblesManager();
        collectiblesManager.ResetCollectiblesData();
    }

    private void ResetTrackElements()
    {
        obstaclesManager.SendAllObjectsToPool();
        collectiblesManager.SendAllObjectToPool();

        playerCarController.ResetData();
    }
}
