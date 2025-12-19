using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class UserDataBehaviour : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private StoreDataBase dataStorer;

    private GameData gameData;
    private UserData userData;

    public GameData GetGameData() => gameData;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<UserDataBehaviour>(this);
    }

    public void InitializeData()
    {
        InitUserData();

        // Debug.unityLogger.logEnabled = false;
    }

    private void InitUserData()
    {
        Debug.Log($"HasSavedUserData: {HasSavedUserData()}");
        if (HasSavedUserData())
        {
            userData = JsonConvert.DeserializeObject<UserData>(PlayerPrefs.GetString(BogeyBlitz_Constants.SaveUserData));
        }
        else
        {
            userData = new UserData();
            userData.userCurrencyData = new UserCurrencyData();
            // // userData.timeData = new TimeData();

            userData.soundData = new InGameSFXData();
            userData.soundData.gameMusicToggle = true;
            userData.soundData.gameSoundToggle = true;

            SaveUserData();
        }
        
        SetFirstUserSessionState(!PlayerPrefs.HasKey(BogeyBlitz_Constants.IsFirstUserSession));
    }

    public UserCurrencyData GetUserCurrencyData()
    {
        return userData.userCurrencyData;
    }

    public InGameSFXData GetSoundData()
    {
        return userData.soundData;
    }

    public void SaveUserCurrencyData(UserCurrencyData userCurrencyData)
    {
        userData.userCurrencyData = userCurrencyData;
        SaveUserData();
    }

    public void SaveSoundData(InGameSFXData sfxData)
    {
        userData.soundData = sfxData;
        SaveUserData();
    }

    #region PLAYER_PREFS_SAVING

    public bool IsFirstUserSession()
    {
        return PlayerPrefs.GetInt(BogeyBlitz_Constants.IsFirstUserSession) == BogeyBlitz_Constants.TRUE;
    }

    public void SetFirstUserSessionState(bool state)
    {
        PlayerPrefs.SetInt(BogeyBlitz_Constants.IsFirstUserSession, state ? BogeyBlitz_Constants.TRUE : BogeyBlitz_Constants.FALSE);
        PlayerPrefs.Save();
    }

    public bool HasSavedUserData()
    {
        return PlayerPrefs.HasKey(BogeyBlitz_Constants.SaveUserData);
    }

    public void SaveUserData()
    {
        PlayerPrefs.SetString(BogeyBlitz_Constants.SaveUserData, JsonConvert.SerializeObject(userData));
        PlayerPrefs.Save();
    }

    public void ClearUserData()
    {
        PlayerPrefs.SetString(BogeyBlitz_Constants.SaveUserData, "");
        PlayerPrefs.DeleteKey(BogeyBlitz_Constants.SaveUserData);
        PlayerPrefs.DeleteKey(BogeyBlitz_Constants.IsFirstUserSession);
        PlayerPrefs.Save();
    }

    #endregion

    #region OLD_CODE

    // // private void InitGameData()
    // // {
    // //     if (gameData == null)
    // //     {
    // //         gameData = new GameData();
    // //         gameData = JsonConvert.DeserializeObject<GameData>("");
    // //     }
    // // }

    // // public void SetLastProgressTime(string timeString, string elapsedSeconds)
    // // {
    // //     Debug.Log($"time :: userData.timeData.lastPlayedProgressTime: {timeString}");
    // //     Debug.Log($"time :: userData.timeData.lastElapsedSeconds: {elapsedSeconds}");

    // //     userData.timeData.lastPlayedProgressTime = timeString;
    // //     userData.timeData.lastElapsedSeconds = elapsedSeconds;
    // //     SaveUserData();
    // // }

    // // public TimeData GetTimeData()
    // // {
    // //     return userData.timeData;
    // // }

    // // public void SetUserHealthData(UserHealthData userHealthData)
    // // {
    // //     if (userData != null)
    // //     {
    // //         userData.userHealthData = userHealthData;
    // //         SaveUserData();
    // //     }
    // // }

    #endregion
}
