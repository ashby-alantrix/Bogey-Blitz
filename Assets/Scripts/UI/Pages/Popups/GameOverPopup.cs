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
        popupManager.GameManager.OnGameStateChange(GameState.GameRestart);
        popupManager.HidePopup(popupType);
    }

    public void OnClick_Options()
    {
        popupManager.HidePopupExplicitly(popupType);
        popupManager.ShowPopup(PopupType.Options);
    }

    public void OnClick_Menu()
    {
        popupManager.HidePopup(popupType);
        popupManager.GameManager.OnGameStateChange(GameState.GameMenu);   
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