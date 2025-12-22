using System.Numerics;
using System;
using DG.Tweening;
using UnityEngine;

public enum PopupType
{
    FTUE_Popup, //
    SettingsPopup, 
    RestartPopup, //
    LevelCompletePopup, 
    GameOverPopup, 
    LevelFailPopup,
    GetMoreLivesPopup, //
    FreeRefillPopup,
    FeedbackPopup,
    TargetGoalPopup
}

public enum PopupScalerType
{
    None,
    Zoom,
    Fade
}

public class PopupBase : UIBase, IUIBase
{
    [SerializeField] protected PopupType popupType;

    private Action<PopupResultEvent> onComplete;
    public PopupType PopupType => popupType;

    protected PopupManager popupManager;
    private UIScaler uiScaler;

    public override void Show()
    {
        if (uiScaler)
            uiScaler.ApplyEffectOnShow(() => base.Show());
        else 
            base.Show();
    }

    public override void Hide()
    {
        if (uiScaler)
            uiScaler.ApplyEffectOnHide(() => base.Hide());
        else 
            base.Hide();
    }

    public void Initialize()
    {
        popupManager = popupManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>() : popupManager;
        popupManager.RegisterPopup(this);
    }

    public void InitNextActionEvent(Action<PopupResultEvent> onComplete)
    {
        this.onComplete = onComplete;
    }

    protected void OnComplete(PopupResultEvent popupResultEvent)
    {
        onComplete?.Invoke(popupResultEvent);
    }

    private void Awake()
    {
        uiScaler = GetComponent<UIScaler>();
    }

    private void OnDestroy()
    {
        onComplete = null;
    }
}
