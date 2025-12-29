using System.Numerics;
using System;
using DG.Tweening;
using UnityEngine;

public enum PopupType
{
    Pause,
    GameOver,
    Options,
    HighScore,
    Instruction
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
    [SerializeField] protected bool hasBackOption = false;

    private Action<PopupResultEvent> onComplete;
    public PopupType PopupType => popupType;
    public bool HasBackOption => hasBackOption;

    protected PopupManager popupManager;
    private UIScaler uiScaler;

    public override void Show()
    {
        SetUIScaler();
        if (uiScaler)
            uiScaler.ApplyEffectOnShow(() => base.Show());
        else
            base.Show();
    }

    private void SetUIScaler()
    {
        uiScaler = uiScaler == null ? GetComponent<UIScaler>() : uiScaler;
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

    private void OnDestroy()
    {
        onComplete = null;
    }
}
