using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public enum TrackCollectibleType
{
    Currency = 0
}

public class CollectiblesManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private float collectibleTimerLimit = 0.1f;
    [SerializeField] private float collectibleSetSpawnDelay = 1f;
    [SerializeField] private float spawnIntervalTime = 1f;
    [SerializeField] private Transform collectibleEndpoint;

    private ObjectPoolManager objectPoolManager;
    private WorldSpawnManager environmentSpawnManager;
    private AIPathManager aiPathManager;
    public SoundManager SoundManager
    {
        get;
        private set;
    }

    // private PowerupsManager powerupsManager;
    private TimerSystem spawnerTimerSystem;
    private TimerSystem stopperTimerSystem;
    private TimerSystem delayTimerSystem;

    public int CollectibleCoins { get; private set; }

    public InGameUIManager InGameUIManager
    {
        get;
        private set;
    }

    public Vector3 CollectibleEndpoint => collectibleEndpoint.position;

    public TrackCollectibleType TrackCollectibleType
    {
        get;
        private set;
    }

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<CollectiblesManager>(this);
    }

    public void InitializeData()
    {
        aiPathManager = InterfaceManager.Instance?.GetInterfaceInstance<AIPathManager>();
        // powerupsManager = InterfaceManager.Instance?.GetInterfaceInstance<PowerupsManager>();
        objectPoolManager = InterfaceManager.Instance?.GetInterfaceInstance<ObjectPoolManager>();
        environmentSpawnManager = InterfaceManager.Instance?.GetInterfaceInstance<WorldSpawnManager>();
        SoundManager = InterfaceManager.Instance?.GetInterfaceInstance<SoundManager>();

        SetInGameUIManager();
        InitializeTimerSystem();
    }

    private void SetInGameUIManager()
    {
        InGameUIManager = InterfaceManager.Instance?.GetInterfaceInstance<InGameUIManager>();
    }

    private void InitializeTimerSystem()
    {
        stopperTimerSystem = new TimerSystem();
        spawnerTimerSystem = new TimerSystem();
        delayTimerSystem = new TimerSystem();

        InitTimers();
    }

    private void InitTimers()
    {
        InitializeStopperTimer();
        InitializePathTimer();
        InitializeDelayTimer();
    }

    private void InitializeDelayTimer()
    {
        delayTimerSystem.Init(collectibleSetSpawnDelay, onComplete: () =>
        {
            InitializeStopperTimer();
            InitializePathTimer();
        });
    }

    private void InitializeStopperTimer()
    {
        stopperTimerSystem.Init(spawnIntervalTime);
    }

    private void InitializePathTimer()
    {
        spawnerTimerSystem.Init(collectibleTimerLimit,
        onComplete: () =>
        {
            OnCollectibleTimerComplete();
        });
    }

    public void SpawnCollectible(Vector3 pos)
    {
        CollectibleBase collectibleBase = objectPoolManager.GetObjectFromPool<CollectibleBase>($"{TrackCollectibleType}", PoolType.Currency);

        collectibleBase.transform.position = pos; //new Vector3(pos.x, 0.5f, pos.y);
        collectibleBase.gameObject.SetActive(true);
    }

    public void SendAllObjectToPool()
    {
        var collectibleBases = FindObjectsOfType<CollectibleBase>();
        Debug.Log($"bases Count: {collectibleBases.Length}");
        foreach (var collectibleBase in collectibleBases)
        {
            collectibleBase.gameObject.SetActive(false);
            SendObjectToPool(collectibleBase);
        }
    }

    public void SendObjectToPool(CollectibleBase collectibleBase)
    {
        objectPoolManager.PassObjectToPool($"{collectibleBase.CollectibleType}", GetPoolType(), collectibleBase);
    }

    public PoolType GetPoolType()
    {
        switch (TrackCollectibleType)
        {
            case TrackCollectibleType.Currency:
                return PoolType.Currency;

            default:
                return PoolType.MAX;
        }
    }

    private void Update()
    {
        if (spawnerTimerSystem == null) return;

        stopperTimerSystem.UpdateTimer(Time.deltaTime);
        if (!stopperTimerSystem.IsTimerComplete)
            spawnerTimerSystem.UpdateTimer(Time.deltaTime);
        else
            delayTimerSystem.UpdateTimer(Time.deltaTime);
    }

    private void OnCollectibleTimerComplete()
    {
        Debug.Log($"CreateCollectibleElements");
        aiPathManager.CreateCollectibleElements();
    }

    public void UpdateCoins()
    {
        Debug.Log($"#### Collectible coins: {CollectibleCoins}");
        CollectibleCoins += 1;
        SetInGameUIManager();
        InGameUIManager.UpdateHUDCoins($"{CollectibleCoins}");
    }

    public void ResetCollectiblesData()
    {
        CollectibleCoins = 0;
        SetInGameUIManager();
        InGameUIManager.UpdateHUDCoins($"{CollectibleCoins}");
    }
}
