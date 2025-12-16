using UnityEngine;

public class CollectibleObjectPool : ObjectPoolBase<CollectibleBase> 
{
    [SerializeField] protected TrackCollectibleType objectType;
    public TrackCollectibleType GetPoolObjectType() => objectType;// goodsType;

    public override void InitPoolFirstTime()
    {
        base.InitPoolFirstTime();
    }
}