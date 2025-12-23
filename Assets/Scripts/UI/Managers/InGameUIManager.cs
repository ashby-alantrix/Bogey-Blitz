using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoBehaviour, IBootLoader,  IBase, IDataLoader
{
    private InGameHUDScreen inGameHUDScreen;

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

    private void SetInGameHUDScreen()
    {
        inGameHUDScreen = inGameHUDScreen == null ? ScreenManager.GetScreen<InGameHUDScreen>(ScreenType.InGameHUDScreen) : inGameHUDScreen;
    }
}
