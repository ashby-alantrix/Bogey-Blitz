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

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<AIController>(this);
    }

    public void InitializeData()
    {
        aiPathManager = InterfaceManager.Instance?.GetInterfaceInstance<AIPathManager>();

        aiPathManager.StartCreatingPathElements();
    }

    void Update()
    {
        transform.position += Vector3.forward * Time.deltaTime * moveSpeed;
    }

    public void ChangeLane(Vector3 pos)
    {
        transform.DOMove(pos, laneChangeTime);
    }
}
