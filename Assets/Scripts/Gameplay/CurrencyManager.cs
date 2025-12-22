using Unity.VisualScripting;
using UnityEngine;

public class CurrencyManager : MonoBehaviour, IBootLoader, IBase, IDataLoader
{
    public UserCurrencyData userCurrencyData;
    private InGameUIManager inGameUIManager;
    private UserDataBehaviour userDataBehaviour;
    private ScreenManager screenManager;
    private InGameHUDScreen inGameHudScreen;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<CurrencyManager>(this);

        userDataBehaviour  = InterfaceManager.Instance?.GetInterfaceInstance<UserDataBehaviour>();
    }

    public void InitializeData()
    {
        Debug.Log($"USERDATA: Initialize Data for currency system");

        userCurrencyData = userDataBehaviour.GetUserCurrencyData();

        screenManager = InterfaceManager.Instance?.GetInterfaceInstance<ScreenManager>();
        inGameHudScreen = screenManager.GetScreen<InGameHUDScreen>(ScreenType.InGameHUDScreen);

        Debug.Log($"userCurrencyData.attainedCurrency: {userCurrencyData.attainedCurrency}");
        // inGameHudScreen.UpdateCurrencyText($"{userCurrencyData.attainedCurrency}");

        Debug.Log($"userDataBehaviour.IsFirstUserSession(): {userDataBehaviour.IsFirstUserSession()}");
    }

    public void AddCurrency(int addAmt)
    {
        userCurrencyData.attainedCurrency += addAmt;
        UpdateCurrencyData();
    }

    public void WithdrawCurrency(int withdrawAmt)
    {
        userCurrencyData.attainedCurrency -= withdrawAmt;
        UpdateCurrencyData();
    }

    public void UpdateCurrencyData()
    {
        Debug.Log($"Updated currency data userCurrencyData.attainedCurrency: {userCurrencyData.attainedCurrency}");
        // inGameHudScreen.UpdateCurrencyText($"{userCurrencyData.attainedCurrency}");
        userDataBehaviour.SaveUserCurrencyData(userCurrencyData);
    }

    public bool HasEnoughCurrency(int availCurrency)
    {
        Debug.Log($"HasEnoughCurrency: {availCurrency} <= {userCurrencyData.attainedCurrency}");
        return availCurrency <= userCurrencyData.attainedCurrency;
    }
}
