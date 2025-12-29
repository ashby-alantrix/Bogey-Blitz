using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighScorePopup : PopupBase
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI coinsCollectedText;

    public void InitInfo(int highScore, int coinsCollected)
    {
        highScoreText.text = $"{highScore}";
        coinsCollectedText.text = $"COINS COLLECTED: {coinsCollected}";
    }

    private void OnClick_Close()
    {
        popupManager.HidePopup(popupType);
        popupManager.ShowPopup(PopupType.GameOver);
    }

    private void OnEnable()
    {
        closeBtn.onClick.AddListener(OnClick_Close);
    }

    private void OnDisable()
    {
        closeBtn.onClick.RemoveAllListeners();
    }
}
