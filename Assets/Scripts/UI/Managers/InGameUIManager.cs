using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoBehaviour, IBootLoader,  IBase, IDataLoader
{
    private ScreenManager screenManager;
    private PopupManager popupManager;
    private InGameHUDScreen inGameHUDScreen;
    private GameOverPopup gameOverPopup;

    public InGameHUDScreen InGameHUDScreen => inGameHUDScreen;
    public GameOverPopup GameOverPopup => gameOverPopup;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<InGameUIManager>(this);
    }

    public void InitializeData()
    {
        screenManager = InterfaceManager.Instance?.GetInterfaceInstance<ScreenManager>();
        popupManager = InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>();

        inGameHUDScreen = screenManager.GetScreen<InGameHUDScreen>(ScreenType.InGameHUDScreen);
        gameOverPopup = popupManager.GetPopup<GameOverPopup>(PopupType.GameOverPopup);

        Debug.Log($"InGameUIManager: InGameHudScreen: {inGameHUDScreen}");
    }
}
