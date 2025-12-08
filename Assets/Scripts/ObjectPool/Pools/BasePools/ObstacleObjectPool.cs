using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleObjectPool : ObjectPoolBase<ObstacleBase>
{
    [SerializeField] protected ObstacleType obstacleType;
    public ObstacleType GetPoolObjectType() => obstacleType;// goodsType;

    public override void InitPoolFirstTime()
    {
        base.InitPoolFirstTime();
    }
}
