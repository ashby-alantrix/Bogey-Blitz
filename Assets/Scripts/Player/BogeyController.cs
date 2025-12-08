using System;
using DG.Tweening;
using UnityEngine;

public class BogeyController : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private Vector3 dir;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float laneWidth;
    [SerializeField] private float laneChangeSpeed;

    [SerializeField] private Transform middleLane;
    [SerializeField] private Transform rightLane;

    private Vector3 targetPosition = Vector3.zero;

    private InputController inputController;
    private BogeyCollisionHandler bogeyCollisionHandler;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<BogeyController>(this);
    }

    public void InitializeData()
    {
        inputController = InterfaceManager.Instance?.GetInterfaceInstance<InputController>();
        Debug.Log($"initialized input controller: {inputController}");
    }

    public void MoveLeft()
    {
        targetPosition = transform.position + (Vector3.left * laneWidth) + (Vector3.forward * 3f);

        transform.DOMove(targetPosition, 0.2f);
    }

    public void MoveRight()
    {
        targetPosition = transform.position + (Vector3.right * laneWidth) + (Vector3.forward * 3f);

        transform.DOMove(targetPosition, 0.2f);

        Debug.Log($"Target position: {targetPosition}");
    }

    private void Start()
    {
        dir = Vector3.forward;    

        bogeyCollisionHandler = GetComponent<BogeyCollisionHandler>();
        laneWidth = (middleLane.position - rightLane.position).magnitude;
    }

    private void Update()
    {
        transform.position += dir * Time.deltaTime * moveSpeed;
    }
}
