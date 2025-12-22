using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AIController : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float laneChangeTime;
    
    private AIPathManager aiPathManager;
    private bool canMove = false;

    public AIPathManager AIPathManager => aiPathManager;

    private TimerSystem timerSystem;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<AIController>(this);
    }

    public void InitializeData()
    {
        aiPathManager = InterfaceManager.Instance?.GetInterfaceInstance<AIPathManager>();
    }

    public void ChangeLane(Vector3 pos)
    {
        transform.DOMove(pos, laneChangeTime);
    }
}
