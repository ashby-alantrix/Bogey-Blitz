using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    Straight_Track = 0,
    Curved_Track = 1,
}

public class BlockBase : ObjectBase
{
    [SerializeField] private BlockType blockType;

    public BlockType BlockType => blockType;
}
