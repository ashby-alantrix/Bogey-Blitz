using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScreenResultEvent
{
    None,
    OnSettingsClicked
}

public class ScreenManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    private Dictionary<ScreenType, ScreenBase> screensDict = new Dictionary<ScreenType, ScreenBase>();

    private ScreenBase activeScreen = null;
    private Stack<ScreenBase> screenBasesStack = new Stack<ScreenBase>();

    public GameManager GameManager
    {
        get;
        private set;
    }
    
    public ScreenBase GetActiveScreen() => activeScreen;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<ScreenManager>(this);
        screensDict.Clear();
    }
    
    public void InitializeData()
    {
        GameManager = InterfaceManager.Instance?.GetInterfaceInstance<GameManager>();
    }

    public void RegisterScreen(ScreenBase screenBase)
    {
        if (!screensDict.ContainsKey(screenBase.ScreenType))
            screensDict.Add(screenBase.ScreenType, screenBase);
        else
            screensDict[screenBase.ScreenType] = screenBase;
    }

    public T GetScreen<T>(ScreenType screenType) where T : ScreenBase
    {
        return screensDict.ContainsKey(screenType) ? (T)screensDict[screenType] : null;
    }

    public void ShowScreen(ScreenType screenType)
    {
        Debug.Log($"ShowScreen :: {screenType}");
        activeScreen = screensDict[screenType];
        if (activeScreen != null)
        {
            screenBasesStack.Push(activeScreen);
            activeScreen.Show();
        }
    }

    public void HideScreen(ScreenType screenType)
    {
        Debug.Log($"HideScreen :: {screenType}");
        if (screensDict[screenType] != null)
        {
            screenBasesStack.Pop();
            Debug.Log($"ScreenManager check: screenBasesStack.Count: {screenBasesStack.Count}");
            activeScreen = screenBasesStack.Count > 0 ? screenBasesStack.Peek() : null;
            screensDict[screenType].Hide();
        }
    }

    public void HideAllScreens()
    {
        foreach (var screenPair in screensDict)
        {
            screenPair.Value.Hide();
        }    
    }
}
