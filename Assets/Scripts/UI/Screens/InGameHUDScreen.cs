using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameHUDScreen : ScreenBase
{
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private Button settingsButton;

    private void OnEnable()
    {
        settingsButton.onClick.AddListener(OnClick_SettingsButton);
    }

    private void OnDisable()
    {
        settingsButton.onClick.RemoveAllListeners();    
    }

    public void UpdateDistanceText(string distanceText)
    {
        this.distanceText.text = distanceText;
    }

    public void UpdateCoinsText(string coinsText)
    {
        this.coinsText.text = coinsText;
    }

    private void OnClick_SettingsButton()
    {
        screenManager.GameManager.OnGameStateChange(GameState.GamePaused);
    }
}
