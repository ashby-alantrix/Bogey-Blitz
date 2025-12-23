using UnityEngine;
using UnityEngine.UI;

public class PausePopup : PopupBase
{
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button optionsBtn;
    [SerializeField] private Button menuBtn;

    public void OnClick_Resume()
    {
        popupManager.HidePopup(popupType);
        popupManager.GameManager.OnGameStateChange(GameState.GameInProgress);
    }

    public void OnClick_Restart()
    {
        popupManager.GameManager.OnGameStateChange(GameState.GameRestart);
        popupManager.HidePopup(popupType);
    }

    public void OnClick_Options()
    {
        /// <summary>
        /// 
        /// -> show the options panel which would contain the sound options
        /// 
        /// </summary>
        
        popupManager.HidePopupExplicitly(popupType);
        popupManager.ShowPopup(PopupType.Options);
    }

    public void OnClick_Menu()
    {
        /// <summary>
        /// 
        /// -> reset the gameplay like when clicking restart function
        /// -> show the main menu screen
        /// 
        /// </summary>
        
        popupManager.HidePopup(popupType);
        popupManager.GameManager.OnGameStateChange(GameState.GameMenu);
    }

    private void OnEnable()
    {
        resumeBtn.onClick.AddListener(OnClick_Resume);
        restartBtn.onClick.AddListener(OnClick_Restart);
        optionsBtn.onClick.AddListener(OnClick_Options);
        menuBtn.onClick.AddListener(OnClick_Menu);
    }

    private void OnDisable()
    {
        resumeBtn.onClick.RemoveAllListeners();
        restartBtn.onClick.RemoveAllListeners();
        optionsBtn.onClick.RemoveAllListeners();
        menuBtn.onClick.RemoveAllListeners();
    }
}
