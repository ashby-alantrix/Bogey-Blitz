
/************************* GAME DATA *************************/
[System.Serializable]
public class GameData
{
    
}


/************************* USER DATA *************************/

[System.Serializable]
public class UserData
{
    public UserCurrencyData userCurrencyData;
    // // public TimeData timeData;
    public InGameSFXData soundData;
}

// // [System.Serializable]
// // public class TimeData
// // {
// //     public string lastPlayedProgressTime;
// //     public string lastElapsedSeconds;
// // }

[System.Serializable]
public class UserCurrencyData
{
    public int attainedCurrency;
}

[System.Serializable]
public class InGameSFXData
{
    public bool gameMusicToggle;
    public bool gameSoundToggle;
}