
public class PopupBackHandler
{
    private PopupManager popupManager;

    public PopupBackHandler(PopupManager popupManager)
    {
        this.popupManager = popupManager;
    }

    public void HandleBackClick(PopupType popupType)
    {
        popupManager.HidePopup(popupType);
        if (popupManager.GetPrevActivePU())
        {
            popupManager.ShowPopup(popupManager.GetPrevActivePU().PopupType);
            popupManager.ResetPrevActionPU();
        }
    }
}