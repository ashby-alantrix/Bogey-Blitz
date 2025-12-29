using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoBehaviour, IBootLoader,  IBase, IDataLoader
{
    private InGameHUDScreen inGameHUDScreen;
    private GameManager gameManager;

    public ScreenManager ScreenManager
    {
        get;
        private set;
    }
    
    public PopupManager PopupManager
    {
        get;
        private set;
    }

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<InGameUIManager>(this);
    }

    public void InitializeData()
    {
        gameManager = InterfaceManager.Instance?.GetInterfaceInstance<GameManager>();
        ScreenManager = InterfaceManager.Instance?.GetInterfaceInstance<ScreenManager>();
        PopupManager = InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>();
    }

    public void UpdateHUDDistance(string distance)
    {
        SetInGameHUDScreen();
        inGameHUDScreen.UpdateDistanceText(distance);
    }

    public void UpdateHUDCoins(string coins)
    {
        SetInGameHUDScreen();
        inGameHUDScreen.UpdateCoinsText(coins);
    }

    public void ResetUIData()
    {
        if (!inGameHUDScreen) return;
        
        inGameHUDScreen.UpdateDistanceText("0");
        inGameHUDScreen.UpdateCoinsText("0");
    }

    private void SetInGameHUDScreen()
    {
        inGameHUDScreen = inGameHUDScreen == null ? ScreenManager.GetScreen<InGameHUDScreen>(ScreenType.InGameHUDScreen) : inGameHUDScreen;
    }

    public void ShowInstructionPopup()
    {
        PopupManager.ShowPopup(PopupType.Instruction);
        Invoke(nameof(HideInstructionPopup), 2f);
    }

    public void HideInstructionPopup()
    {
        PopupManager.HidePopup(PopupType.Instruction);
        gameManager.OnGameStateChange(GameState.GameStart);
    }
}
