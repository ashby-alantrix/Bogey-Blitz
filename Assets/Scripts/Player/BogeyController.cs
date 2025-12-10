using System;
using DG.Tweening;
using UnityEngine;

public class BogeyController : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private Transform leftBound;
    [SerializeField] private Transform rightBound;

    [SerializeField] private Vector3 dir;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float laneWidth;
    [SerializeField] private float laneChangeTime;
    [SerializeField] private float laneChangeSpeed;

    [SerializeField] private Transform middleLane;
    [SerializeField] private Transform rightLane;

    private Vector3 targetPosition = Vector3.zero;
    private bool isChangingLane = false;

    private InputController inputController;
    private BogeyCollisionHandler bogeyCollisionHandler;
    public BogeyCollisionHandler BogeyCollisionHandler => bogeyCollisionHandler;

    private EnvironmentSpawnManager environmentSpawnManager;
    public EnvironmentSpawnManager EnvironmentSpawnManager => environmentSpawnManager;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<BogeyController>(this);
    }

    public void InitializeData()
    {
        inputController = InterfaceManager.Instance?.GetInterfaceInstance<InputController>();
        environmentSpawnManager = InterfaceManager.Instance?.GetInterfaceInstance<EnvironmentSpawnManager>();

        Debug.Log($"initialized input controller: {inputController}");
    }

    public void UpdateMovement(Vector2 swipeDelta)
    {
        if (isChangingLane) return;
        
        if (swipeDelta.x > 0 && transform.position.x < rightBound.position.x)
        {
            isChangingLane = true;
            MoveRight();
        }
        else if (swipeDelta.x < 0 && transform.position.x > leftBound.position.x)
        {
            isChangingLane = true;
            MoveLeft();
        }
    }

    public void MoveLeft()
    {
        targetPosition = transform.position + (Vector3.left * laneWidth);// + (Vector3.forward * 3f);

        transform.DOMove(targetPosition, laneChangeTime).OnComplete(() => 
        {
            isChangingLane = false;
        });
    }

    public void MoveRight()
    {
        targetPosition = transform.position + (Vector3.right * laneWidth);// + (Vector3.forward * 3f);

        transform.DOMove(targetPosition, laneChangeTime).OnComplete(() => 
        {
            isChangingLane = false;
        });

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
        // transform.position += dir * Time.deltaTime * moveSpeed;
    }
}
