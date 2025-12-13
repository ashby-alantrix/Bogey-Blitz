using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBase : ObjectBase
{
    [SerializeField] private TrackObstacleType objectType;
    [SerializeField] private Transform endPoint;

    private ObstaclesManager obstaclesManager;

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

    void Update()
    {
        if (!obstaclesManager)
        {
            obstaclesManager = InterfaceManager.Instance.GetInterfaceInstance<ObstaclesManager>();    
        }

        if (obstaclesManager && transform.position.z < obstaclesManager.ObstacleEndpoint.z)
        {
            Debug.Log($"Deactivating object :: {Mathf.Abs(transform.position.z)} > {Mathf.Abs(obstaclesManager.ObstacleEndpoint.z)}");
            gameObject.SetActive(false);
            obstaclesManager.SendObjectToPool(this);
        }
    }
}
