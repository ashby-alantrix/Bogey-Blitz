using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public enum TrackCollectibleType
{
    Currency = 0,
    Powerup1 = 1,
    Powerup2 = 2,
    Powerup3 = 3
}

public class CollectiblesManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private float collectibleTimerLimit = 0.1f;
    [SerializeField] private float collectibleSetSpawnDelay = 1f;
    [SerializeField] private float spawnIntervalTime = 1f;
    [SerializeField] private Transform collectibleEndpoint;

    private ObjectPoolManager objectPoolManager;
    private EnvironmentSpawnManager environmentSpawnManager;
    private AIPathManager aiPathManager;

    private bool hasDelay = true;
    private TimerSystem spawnerTimerSystem;
    private TimerSystem stopperTimerSystem;
    private TimerSystem delayTimerSystem;

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
        objectPoolManager = InterfaceManager.Instance?.GetInterfaceInstance<ObjectPoolManager>();
        environmentSpawnManager = InterfaceManager.Instance?.GetInterfaceInstance<EnvironmentSpawnManager>();

        InitializeTimerSystem();
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
        collectibleBase.SpawnableMoverBase.InitMoveSpeed(environmentSpawnManager.EnvironmentMoveSpeed);
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

            case TrackCollectibleType.Powerup1:
            case TrackCollectibleType.Powerup2:
            case TrackCollectibleType.Powerup3:
                return PoolType.Powerup;

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
}
