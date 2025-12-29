using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    CarCrash,
    CarGearChange,
    GiftCollectible,
    Button_Click
}

[System.Serializable]
public class SoundData
{
    public SoundType soundType;
    public int priority;
    public AudioClip soundClip;
}

public enum SingleInstAudioSourceType
{
    BG,
    CarAccel
}

public enum MultiAudioSourceType
{
    MovableTrain
}

[System.Serializable]
public class AudioSourceData
{
    public SingleInstAudioSourceType sourceType;
    public AudioSource audioSource;
}

public class SoundManager : MonoBehaviour, IBootLoader, IBase, IDataLoader
{
    // [SerializeField] private AudioSourceData[] singleInstAudioSourceDatas;
    [SerializeField] private AudioSource audioClipSource;
    [SerializeField] private AudioSource bgAudioSource;
    [SerializeField] private AudioSource carAccelAudioSource;

    [SerializeField] private SoundData[] soundDatas;

    private Queue<AudioSource> multiSourcesQueue = new Queue<AudioSource>(); // only a single multiSource right now
    private Dictionary<SoundType, SoundData> soundDataDict = new Dictionary<SoundType, SoundData>();
    // private Dictionary<SingleInstAudioSourceType, AudioSource> singleInstAudioSourcesDict = new Dictionary<SingleInstAudioSourceType, AudioSource>();

    private UserDataBehaviour userDataBehaviour;
    private InGameSFXData inGameSFXData;
    private PopupManager popupManager;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<SoundManager>(this);
    }

    public void InitializeData()
    {
        userDataBehaviour = InterfaceManager.Instance?.GetInterfaceInstance<UserDataBehaviour>();
        popupManager = InterfaceManager.Instance?.GetInterfaceInstance<PopupManager>();
        
        for (int idx = 0; idx < soundDatas.Length; idx++)
        {
            if (soundDataDict.ContainsKey(soundDatas[idx].soundType))
                soundDataDict[soundDatas[idx].soundType] =  soundDatas[idx];
            else 
                soundDataDict.Add(soundDatas[idx].soundType, soundDatas[idx]);
        }

        // foreach (var data in singleInstAudioSourceDatas)
        // {
        //     singleInstAudioSourcesDict.Add(data.sourceType, data.audioSource);
        // }        

        inGameSFXData = userDataBehaviour.GetSoundData();

        SetGameMusic(inGameSFXData.gameMusicToggle);
        SetGameSound(inGameSFXData.gameSoundToggle);

        Debug.Log($"#### popupManager.GetPopup<OptionsPopup>(PopupType.Options): {popupManager.GetPopup<OptionsPopup>(PopupType.Options)}");
        popupManager.GetPopup<OptionsPopup>(PopupType.Options).InitSFXData(inGameSFXData, this);
    }

    public void SetGameMusic(bool state)
    {
        inGameSFXData.gameMusicToggle = state;
        userDataBehaviour.SaveSoundData(inGameSFXData);
    }

    public void SetGameSound(bool state)
    {
        inGameSFXData.gameSoundToggle = state;
        userDataBehaviour.SaveSoundData(inGameSFXData);
    }

    public void RegisterToMultiSources(AudioSource audioSource)
    {
        Debug.Log($"Registering to multisources queue");
        multiSourcesQueue.Enqueue(audioSource);
    }

    public void UnregisterFromMultiSources()
    {
        if (multiSourcesQueue.Count < 1) return;    

        Debug.Log($"Unregistering from multisources queue");
        multiSourcesQueue.Dequeue();
    }

    public void SetMultiSourcesState(bool state)
    {
        Debug.Log($"source :: {multiSourcesQueue.Count}");
        if (state && inGameSFXData.gameSoundToggle)
        {
            foreach (var source in multiSourcesQueue)
                source.Play();
        }
        else
        {
            foreach (var source in multiSourcesQueue)
                source.Stop();
        }
    }

    public void PlayPrimaryGameSoundClip(SoundType soundType)
    {
        if (!enabled || !inGameSFXData.gameSoundToggle) return;

        SoundData soundData = soundDataDict[soundType];

        audioClipSource.clip = soundData.soundClip;
        audioClipSource.PlayOneShot(soundData.soundClip);
    }

    public void PlayButtonSoundClip(SoundType soundType)
    {
        if (!enabled || !inGameSFXData.gameSoundToggle) return;

        SoundData soundData = soundDataDict[soundType];
        audioClipSource.PlayOneShot(soundData.soundClip);
    }

    public void PlayCarAudio(bool state)
    {
        Debug.Log($"Play car audio");
        if (!enabled || !inGameSFXData.gameSoundToggle || !state)
        {
            carAccelAudioSource.Stop();
            return;
        }

        carAccelAudioSource.Play();
    }

    public void PlayBGAudio(bool state)
    {
        if (!enabled || !inGameSFXData.gameMusicToggle || !state)
        {
            bgAudioSource.Stop();
            return;
        }

        bgAudioSource.Play();
    }
}