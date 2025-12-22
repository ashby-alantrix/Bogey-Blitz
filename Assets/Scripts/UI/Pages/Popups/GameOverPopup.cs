using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPopup : PopupBase
{
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button optionsBtn;
    [SerializeField] private Button menuBtn;

    public void OnClick_Restart()
    {
        if (!popupManager) Debug.LogError($"Popupmanager is null");
        if (!popupManager.GameManager) Debug.LogError($"game manager is null");

        popupManager.GameManager.OnGameStateChange(GameState.GameRestart);
        popupManager.HidePopup(popupType);
    }

    public void OnClick_Options()
    {
        
    }

    public void OnClick_Menu()
    {
        
    }

    private void OnEnable()
    {
        restartBtn.onClick.AddListener(OnClick_Restart);
        optionsBtn.onClick.AddListener(OnClick_Options);
        menuBtn.onClick.AddListener(OnClick_Menu);
    }

    private void OnDisable()
    {
        restartBtn.onClick.RemoveAllListeners();
        optionsBtn.onClick.RemoveAllListeners();
        menuBtn.onClick.RemoveAllListeners();
    }

}