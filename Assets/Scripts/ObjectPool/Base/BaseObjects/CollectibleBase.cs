using UnityEngine;

public class CollectibleBase : ObjectBase
{
    [SerializeField] private TrackCollectibleType collectibleType;

    private ObjectPoolManager objectPoolManager;
    private CollectiblesManager collectiblesManager;

    public TrackCollectibleType CollectibleType => collectibleType;

    public SpawnableMoverBase SpawnableMoverBase
    {
        get;
        private set;
    }

    private void Awake()
    {
        SpawnableMoverBase = GetComponent<SpawnableMoverBase>();
    }

    private void OnTriggerEnter(Collider other)
    {
        objectPoolManager = objectPoolManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<ObjectPoolManager>() : objectPoolManager;

        switch (other.gameObject.tag)
        {
            case BogeyBlitz_Constants.PLAYER_TAG:
                gameObject.SetActive(false);
                objectPoolManager.PassObjectToPool($"{TrackCollectibleType.Currency}", PoolType.Currency, this);
            break;
        }
    }

    private void Update()
    {
        if (!collectiblesManager)
        {
            collectiblesManager = InterfaceManager.Instance.GetInterfaceInstance<CollectiblesManager>();    
        }

        if (collectiblesManager && transform.position.z < collectiblesManager.CollectibleEndpoint.z)
        {
            gameObject.SetActive(false);
            collectiblesManager.SendObjectToPool(this);
        }
    }
}
