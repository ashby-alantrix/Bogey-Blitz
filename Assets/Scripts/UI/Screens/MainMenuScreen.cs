using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : ScreenBase
{
    [SerializeField] private Button startBtn;
    [SerializeField] private Button optionsBtn;
    [SerializeField] private Button quitBtn;

    private GameManager gameManager;

    private void OnEnable()
    {
        startBtn.onClick.AddListener(OnClick_StartButton);
        optionsBtn.onClick.AddListener(OnClick_OptionsButton);
        quitBtn.onClick.AddListener(OnClick_QuitButton);
    }

    private void OnClick_StartButton()
    {
        screenManager.HideScreen(ScreenType);
        // start the game from the game manager

        SetGameManager();
        gameManager.OnGameStateChange(GameState.GameStart);
    }

    private void SetGameManager()
    {
        gameManager = gameManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<GameManager>() : gameManager;
    }

    private void OnClick_OptionsButton()
    {
        // show the options popup
        SetGameManager();
                
        screenManager.GameManager.InGameUIManager.PopupManager.ShowPopup(PopupType.Options);
    }

    private void OnClick_QuitButton()
    {
        Application.Quit();
    }
}
