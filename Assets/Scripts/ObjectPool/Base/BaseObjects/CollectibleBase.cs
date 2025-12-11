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

    public CollectibleType CollectibleType => collectibleType;
}
