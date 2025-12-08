using UnityEngine;

public enum ObstacleType
{
    Broken_Rail_Piece = 0,
    Log = 1,
    Parked_Wagon = 2,
}

public class ObstacleBase : ObjectBase
{
    [SerializeField] private ObstacleType obstacleType;

    public ObstacleType ObstacleType => obstacleType;
}
