using UnityEngine;

public enum CollectibleType
{
    Coins = 0,
}

public class CollectibleBase : ObjectBase
{
    [SerializeField] private CollectibleType collectibleType;

    public CollectibleType CollectibleType => collectibleType;
}
