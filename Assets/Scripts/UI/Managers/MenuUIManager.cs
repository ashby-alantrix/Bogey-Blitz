using UnityEngine;

public class MenuUIManager : MonoBehaviour, IBootLoader, IBase
{
    private PopupManager popupManager;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<MenuUIManager>(this);
        popupManager = InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>();
    }
}
