using System;
using System.Collections.Generic;
using UnityEngine;

public enum PopupResultEvent
{
    None,
    OnOptionsClosed,
}

public class PopupManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    private Dictionary<PopupType, PopupBase> popupsDict = new Dictionary<PopupType, PopupBase>();

    private PopupBase activePopup = null;
    private PopupBase prevActivePopup = null;

    public GameManager GameManager
    {
        get; 
        private set;
    }

    public PopupBase GetPrevActivePU() => prevActivePopup;
    public PopupBase GetActivePU() => activePopup;

    private Stack<PopupBase> popupBasesStack = new Stack<PopupBase>();

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<PopupManager>(this);
        popupsDict.Clear();
    }

    public void InitializeData()
    {
        GameManager = InterfaceManager.Instance?.GetInterfaceInstance<GameManager>();
    }

    public void RegisterPopup(PopupBase popupBase)
    {
        if (!popupsDict.ContainsKey(popupBase.PopupType))
            popupsDict.Add(popupBase.PopupType, popupBase);
        else
            popupsDict[popupBase.PopupType] = popupBase;

        // popupBase.InitNextActionEvent((resultType) => OnPopupClosedExceuteEvent(resultType));
    }

    public T GetPopup<T>(PopupType uiType) where T : PopupBase
    {
        return popupsDict.ContainsKey(uiType) ? (T)popupsDict[uiType] : null;
    }

    public void ShowPopup(PopupType uiType)
    {
        activePopup = popupsDict[uiType];
        if (activePopup != null)
        {
            Debug.Log($"Stack check :: activePopup added to stack: {activePopup.PopupType}");
            popupBasesStack.Push(activePopup);
            activePopup.Show();
        }
    }

    public void HidePopup(PopupType popupType)
    {
        if (popupsDict[popupType] != null)
        {
            var poppedElement = popupBasesStack.Pop();
            Debug.Log($"Stack check :: before popupBasesStack: {popupBasesStack?.Count}");
            Debug.Log($"Stack check :: popped element: {poppedElement.PopupType}");

            activePopup = popupBasesStack.Count > 0 ? popupBasesStack.Peek() : null;
            popupsDict[popupType].Hide();

            Debug.Log($"Stack check :: activePopup: {activePopup}");

            Debug.Log($"Stack check :: peeked element type: {activePopup?.PopupType}, hidden element from dict: {popupType}");
            Debug.Log($"Stack check :: after popupBasesStack: {popupBasesStack?.Count}");
        }
    }

    public void HidePopupExplicitly(PopupType popupType)
    {
        if (popupsDict[popupType] != null)
        {
            prevActivePopup = popupsDict[popupType];
            popupsDict[popupType].Hide();
        }
    }

    public void ResetPrevActionPU() => prevActivePopup = null;

    public void HideAllPopups()
    {
        foreach (var popupPair in popupsDict)
        {
            popupPair.Value.Hide();
        }    
    }

    // public void OnPopupClosedExceuteEvent(PopupResultEvent popupResultEvent)
    // {
    //     switch (popupResultEvent)
    //     {
    //         case PopupResultEvent.None:

    //         break;
    //         case PopupResultEvent.OnOptionsClosed:
    //         break;
    //         default:
    //         break;
    //     }
    // }
}
