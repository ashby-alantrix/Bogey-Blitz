using UnityEngine;

public enum ScreenType
{
    InGameHUDScreen,
    MenuHUDScreen
}

public class ScreenBase : UIBase, IUIBase
{
    [SerializeField] protected ScreenType screenType;

    public ScreenType ScreenType => screenType;

    protected ScreenManager screenManager;

    private UIScaler uiScaler;

    public void Initialize()
    {
        screenManager = screenManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<ScreenManager>() : screenManager;
        screenManager.RegisterScreen(this);
    }

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
            uiScaler.ApplyEffectOnShow(() => base.Hide());
        else 
            base.Hide();
    }

    private void Awake()
    {
        uiScaler = GetComponent<UIScaler>();
    }
}
