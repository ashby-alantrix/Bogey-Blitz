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
        base.Hide();
        // start the game from the game manager

        gameManager = InterfaceManager.Instance?.GetInterfaceInstance<GameManager>();
        gameManager.OnGameStateChange(GameState.GameStart);

    }

    private void OnClick_OptionsButton()
    {
        // show the options popup
    }

    private void OnClick_QuitButton()
    {
        Application.Quit();
    }
}
