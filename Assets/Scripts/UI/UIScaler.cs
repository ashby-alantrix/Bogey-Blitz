using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIScaler : MonoBehaviour
{
    [Header("Popup Scaling")]
    [SerializeField] protected Transform popupScaleContent;
    // [SerializeField] protected bool shouldScale = true;
    [SerializeField] protected float zoomDuration = 0.5f;
    [SerializeField] protected PopupScalerType popupScalerType = PopupScalerType.Zoom;
    [SerializeField] protected float fadeDuration;
    
    protected CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ApplyEffectOnShow(Action onComplete)
    {
        switch (popupScalerType)
        {
            case PopupScalerType.None:
                onComplete?.Invoke();
            break;
            case PopupScalerType.Zoom:
                popupScaleContent.localScale = Vector3.zero;
                onComplete?.Invoke();
                popupScaleContent.DOScale(Vector3.one, zoomDuration);
            break;
            case PopupScalerType.Fade:
                canvasGroup.alpha = 0;
                onComplete?.Invoke();
                canvasGroup.DOFade(1, fadeDuration);
            break;
        }
    }

    public void ApplyEffectOnHide(Action onComplete)
    {
        switch (popupScalerType)
        {
            case PopupScalerType.None:
                onComplete?.Invoke();
            break;
            case PopupScalerType.Zoom:
                popupScaleContent.DOScale(Vector3.zero, zoomDuration).OnComplete(() => onComplete?.Invoke());
            break;
            case PopupScalerType.Fade:
                canvasGroup.DOFade(0, fadeDuration).OnComplete(() => onComplete?.Invoke());
            break;
        }
    }
}