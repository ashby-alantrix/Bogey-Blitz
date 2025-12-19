using UnityEngine;
using UnityEngine.UI;

public class RestartPopup : PopupBase
{
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button closeBtn;

    private InputController inputController;
    private ScreenManager screenManager;

    public void OnClick_RestartBtn()
    {
        screenManager = screenManager == null ? InterfaceManager.Instance.GetInterfaceInstance<ScreenManager>() : screenManager;

        // if (healthSystem.AvailableLifes > 1)
        // {
        //     screenManager.GetScreen<InGameHUDScreen>(ScreenType.InGameHUDScreen).ShowSettingDropdown();
        //     popupManager.HidePopup(popupType);
        // }
        // else if (!healthSystem.UserHealthData.haveUsedFreeRefill)
        // {
        //     OnComplete(PopupResultEvent.OnFreeRefillHealth);
        // }
        // else
        // {
        //     OnComplete(PopupResultEvent.FreeRefillUsed);
        // }
    }

    public void OnClick_CloseBtn()
    {
        popupManager.HidePopup(popupType);
    }

    private void OnEnable()
    {
        restartBtn.onClick.AddListener(OnClick_RestartBtn);
        closeBtn.onClick.AddListener(OnClick_CloseBtn);

        inputController = inputController == null ? InterfaceManager.Instance?.GetInterfaceInstance<InputController>() : inputController;
        // inputController?.SetInputState(false);
    }

    private void OnDisable()
    {
        restartBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();

        Invoke(nameof(OnPopupClosed), 0.5f);
    }

    private void OnPopupClosed()
    {
        inputController = inputController == null ? InterfaceManager.Instance?.GetInterfaceInstance<InputController>() : inputController;
        // inputController?.SetInputState(true);
    }
}
