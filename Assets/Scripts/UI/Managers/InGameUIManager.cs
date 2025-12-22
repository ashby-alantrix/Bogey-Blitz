using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoBehaviour, IBootLoader,  IBase, IDataLoader
{
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


}
