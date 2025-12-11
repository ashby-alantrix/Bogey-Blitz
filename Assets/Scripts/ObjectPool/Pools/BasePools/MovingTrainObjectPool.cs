using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrainObjectPool : ObjectPoolBase<ObstacleBase> 
{
    [SerializeField] protected TrackObstacleType objectType;
    public TrackObstacleType GetPoolObjectType() => objectType;// goodsType;

    public override void InitPoolFirstTime()
    {
        base.InitPoolFirstTime();
    }
}
