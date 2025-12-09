using UnityEngine;

public enum TrainType
{
    Train1 = 0,
}

public class TrainBase : ObjectBase
{
    [SerializeField] private TrainType trainType;

    public TrainType TrainType => trainType;
}
