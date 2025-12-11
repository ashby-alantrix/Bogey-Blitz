using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBase : ObjectBase
{
    [SerializeField] private TrackObstacleType objectType;
    [SerializeField] private Transform endPoint;

    public Transform EndPoint => endPoint;
    public TrackObstacleType ObjectType => objectType;
    public bool HasAIPassed
    {
        get;
        private set;
    }

    public void SetAIPassedState(bool state)
    {
        HasAIPassed = state;
    }
}
