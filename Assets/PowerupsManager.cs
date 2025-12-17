using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrackPowerupType
{
    Magnet_Powerup = 0,
    Shield_Powerup = 1,
    Boost_Powerup = 2,
    Freeze_Powerup = 3,
    Revive_Powerup = 4,
    MAX = 5
}

public class PowerupsManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    private List<TrackPowerupType> trackPowerupTypes= new List<TrackPowerupType>();

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<PowerupsManager>(this);
    }

    public void InitializeData()
    {
        
    }

    public void CanSpawnPowerup()
    {
        // isPowerup available

        // Can powerup be spawned/unlocked
        // cache it in an array 
        // Randomly retrieve the powerup
    }
}
