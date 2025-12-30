using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class UserDataBehaviour : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    // [SerializeField] private StoreDataBase dataStorer;

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

        Debug.unityLogger.logEnabled = false;
    }

    private void InitUserData()
    {
        Debug.Log($"HasSavedUserData: {HasSavedUserData()}");
        if (HasSavedUserData())
        {
            userData = JsonConvert.DeserializeObject<UserData>(PlayerPrefs.GetString(ArcticEscape_Constants.SaveUserData));
        }
        else
        {
            userData = new UserData();
            userData.inGameData = new InGameData();
            userData.userCurrencyData = new UserCurrencyData();
            // // userData.timeData = new TimeData();

            userData.soundData = new InGameSFXData();
            userData.soundData.gameMusicToggle = true;
            userData.soundData.gameSoundToggle = true;

            SaveUserData();
        }
        
        SetFirstUserSessionState(!PlayerPrefs.HasKey(ArcticEscape_Constants.IsFirstUserSession));
        ToggleHasSeenInstructionState(PlayerPrefs.HasKey(ArcticEscape_Constants.HasSeenInstruction));
    }

    public InGameData GetInGameData() => userData.inGameData;

    public UserCurrencyData GetUserCurrencyData()
    {
        return userData.userCurrencyData;
    }

    public InGameSFXData GetSoundData()
    {
        return userData.soundData;
    }

    public void SaveInGameData(InGameData inGameData)
    {
        userData.inGameData = inGameData;
        SaveUserData();
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
        return PlayerPrefs.GetInt(ArcticEscape_Constants.IsFirstUserSession) == ArcticEscape_Constants.TRUE;
    }

    public bool HasSeenInstruction()
    {
        return PlayerPrefs.GetInt(ArcticEscape_Constants.HasSeenInstruction) == ArcticEscape_Constants.TRUE;
    }

    public void SetFirstUserSessionState(bool state)
    {
        PlayerPrefs.SetInt(ArcticEscape_Constants.IsFirstUserSession, state ? ArcticEscape_Constants.TRUE : ArcticEscape_Constants.FALSE);
        PlayerPrefs.Save();
    }

    public void ToggleHasSeenInstructionState(bool state)
    {
        PlayerPrefs.SetInt(ArcticEscape_Constants.HasSeenInstruction, state ? ArcticEscape_Constants.TRUE : ArcticEscape_Constants.FALSE);
        PlayerPrefs.Save();
    }

    public bool HasSavedUserData()
    {
        return PlayerPrefs.HasKey(ArcticEscape_Constants.SaveUserData);
    }

    public void SaveUserData()
    {
        PlayerPrefs.SetString(ArcticEscape_Constants.SaveUserData, JsonConvert.SerializeObject(userData));
        PlayerPrefs.Save();
    }

    public void ClearUserData()
    {
        PlayerPrefs.SetString(ArcticEscape_Constants.SaveUserData, "");
        PlayerPrefs.DeleteKey(ArcticEscape_Constants.SaveUserData);
        PlayerPrefs.DeleteKey(ArcticEscape_Constants.IsFirstUserSession);
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
