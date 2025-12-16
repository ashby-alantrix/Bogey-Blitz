using Unity.VisualScripting;
using UnityEngine;

public enum CollectibleType
{
    Coins = 0,
    Powerup1 = 1,
    Powerup2 = 2,
    Powerup3 = 3,
}

public class CollectibleBase : ObjectBase
{
    [SerializeField] private CollectibleType collectibleType;

    private ObjectPoolManager objectPoolManager;

    public CollectibleType CollectibleType => collectibleType;

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
                objectPoolManager.PassObjectToPool($"{TrackCollectibleType.Currency}", PoolType.Collectible, this);
            break;
        }
    }
}
