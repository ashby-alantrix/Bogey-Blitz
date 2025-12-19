using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPopup : PopupBase
{
    [SerializeField] private Button playUsingCurrencyBtn;
    [SerializeField] private TextMeshProUGUI remGoodsToSort;
    [SerializeField] private Button closeBtn;

    private PopupManager popupManager;
    private CurrencyManager currencyManager;

    private int nodesToClear = 5;
    private int clearCurrency = 200;

    new void OnEnable()
    {
        // base.OnEnable();
        playUsingCurrencyBtn.onClick.AddListener(() => OnClick_PlayUsingCurrency());
        closeBtn.onClick.AddListener(() => OnClick_CloseBtn());
    }

    new void OnDisable()
    {
        // base.OnDisable();
        playUsingCurrencyBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
    }

    public void InitData(int remGoods)
    {
        remGoodsToSort.text = $"{remGoods}";
    }

    private void OnClick_PlayUsingCurrency()
    {
        currencyManager = currencyManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<CurrencyManager>() : currencyManager;
        SetPopupManager();

        if (!currencyManager.HasEnoughCurrency(clearCurrency))
        {
            // show feedback message -> not enough coins
            popupManager.GetPopup<FeedbackPopup>(PopupType.FeedbackPopup).SetFeedbackText($"NOT ENOUGH COINS");
            popupManager.ShowPopup(PopupType.FeedbackPopup);
            return;
        }

        currencyManager.WithdrawCurrency(clearCurrency);

        popupManager.HidePopup(popupType);
        OnComplete(PopupResultEvent.None);
    }

    protected void OnClick_CloseBtn()
    {
        SetPopupManager();
        popupManager.HidePopup(popupType);

        OnComplete(PopupResultEvent.LifeLostInGameOver);
    }

    private void SetPopupManager()
    {
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;
    }
}