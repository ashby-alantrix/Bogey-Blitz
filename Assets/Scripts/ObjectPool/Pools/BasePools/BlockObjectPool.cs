using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockObjectPool : ObjectPoolBase<BlockBase> 
{
    [SerializeField] protected BlockType blockType;
    public BlockType GetPoolObjectType() => blockType;// goodsType;

    public override void InitPoolFirstTime()
    {
        base.InitPoolFirstTime();
    }
}
