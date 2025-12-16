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

    private ObjectPoolManager objectPoolManager;
    private EnvironmentSpawnManager environmentSpawnManager;
    private AIPathManager aiPathManager;

    private float collectibleTimer = 0;

    private TimerSystem timerSystem;

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

        timerSystem = new TimerSystem();
        timerSystem.Init(collectibleTimerLimit, () => OnCollectibleTimerComplete());
    }

    public void SpawnCollectible(Vector3 pos)
    {
        CollectibleBase collectibleBase = objectPoolManager.GetObjectFromPool<CollectibleBase>($"{TrackCollectibleType}", PoolType.Collectible);

        collectibleBase.transform.position = pos; //new Vector3(pos.x, 0.5f, pos.y);
        collectibleBase.transform.position = new Vector3(collectibleBase.transform.position.x, 0.5f, collectibleBase.transform.position.z); //new Vector3(pos.x, 0.5f, pos.y);
        collectibleBase.gameObject.SetActive(true);
        collectibleBase.SpawnableMoverBase.InitMoveSpeed(environmentSpawnManager.EnvironmentMoveSpeed);
    }

    private void Update()
    {
        if (timerSystem == null) return;

        timerSystem.UpdateTimer(Time.deltaTime);
    }

    private void OnCollectibleTimerComplete()
    {
        Debug.Log($"CreateCollectibleElements");
        aiPathManager.CreateCollectibleElements();
    }
}
