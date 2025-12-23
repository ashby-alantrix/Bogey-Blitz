using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    CarAcceleration,
    CarCrash,
    CarGearChange,
    GiftCollectible,
    MetroTrainMovable, //
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
    OneShotClip,
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
    [SerializeField] private AudioSourceData[] singleInstAudioSourceDatas;

    [SerializeField] private SoundData[] soundDatas;

    public bool IsGameSoundOn
    {
        get;
        private set;
    }

    private Queue<AudioSource> multiSourcesQueue = new Queue<AudioSource>(); // only a single multiSource right now
    private Dictionary<SoundType, SoundData> soundDataDict = new Dictionary<SoundType, SoundData>();
    private Dictionary<SingleInstAudioSourceType, AudioSource> singleInstAudioSourcesDict = new Dictionary<SingleInstAudioSourceType, AudioSource>();

    private AudioSource secondaryAudioSource;

    private UserDataBehaviour userDataBehaviour;
    private InGameSFXData soundData;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<SoundManager>(this);
    }

    public void InitializeData()
    {
        // userDataBehaviour = InterfaceManager.Instance?.GetInterfaceInstance<UserDataBehaviour>();

        for (int idx = 0; idx < soundDatas.Length; idx++)
        {
            if (soundDataDict.ContainsKey(soundDatas[idx].soundType))
                soundDataDict[soundDatas[idx].soundType] =  soundDatas[idx];
            else 
                soundDataDict.Add(soundDatas[idx].soundType, soundDatas[idx]);
        }

        foreach (var data in singleInstAudioSourceDatas)
        {
            singleInstAudioSourcesDict.Add(data.sourceType, data.audioSource);
        }        

        // soundData = userDataBehaviour.GetSoundData();
        // IsGameSoundOn = soundData.gameSoundToggle;
    }

    public void SetGameSound(bool state)
    {
        IsGameSoundOn = state;
        // soundData.gameSoundToggle = state;
        // userDataBehaviour.SaveSoundData(soundData);
    }

    public void RegisterToMultiSources(AudioSource audioSource)
    {
        multiSourcesQueue.Enqueue(audioSource);
    }

    public void UnregisterFromMultiSources()
    {
        multiSourcesQueue.Dequeue();
    }

    public void PlayPrimaryGameSoundClip(SoundType soundType)
    {
        // if (!enabled || !IsGameSoundOn) return;

        SoundData soundData = soundDataDict[soundType];

        // singleInstAudioSourcesDict[SingleInstAudioSourceType.OneShotClip].priority = soundData.priority;
        singleInstAudioSourcesDict[SingleInstAudioSourceType.OneShotClip].clip = soundData.soundClip;
        singleInstAudioSourcesDict[SingleInstAudioSourceType.OneShotClip].PlayOneShot(soundData.soundClip);

        Debug.Log($"soundData.soundClip: {soundData.soundClip}");
    }

    public void PlayButtonSoundClip(SoundType soundType)
    {
        SoundData soundData = soundDataDict[soundType];

        // singleInstAudioSourcesDict[SingleInstAudioSourceType.OneShotClip].priority = soundData.priority;
        singleInstAudioSourcesDict[SingleInstAudioSourceType.OneShotClip].PlayOneShot(soundData.soundClip);
    }

    public void PlayMusic(SingleInstAudioSourceType singleInstAudioSourceType, bool play)
    {
        if (play)
            singleInstAudioSourcesDict[singleInstAudioSourceType].Play();
        else 
            singleInstAudioSourcesDict[singleInstAudioSourceType].Stop();
    }
}