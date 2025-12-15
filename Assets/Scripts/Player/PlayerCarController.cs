using System;
using DG.Tweening;
using UnityEngine;

public class PlayerCarController : MonoBehaviour, IBase, IBootLoader, IDataLoader
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
    private PlayerCollisionHandler bogeyCollisionHandler;
    
    public PlayerCollisionHandler BogeyCollisionHandler => bogeyCollisionHandler;
    public FollowCamera FollowCamera
    {
        get;
        private set;
    }

    private EnvironmentSpawnManager environmentSpawnManager;
    public EnvironmentSpawnManager EnvironmentSpawnManager => environmentSpawnManager;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<PlayerCarController>(this);
    }

    public void InitializeData()
    {
        inputController = InterfaceManager.Instance?.GetInterfaceInstance<InputController>();
        environmentSpawnManager = InterfaceManager.Instance?.GetInterfaceInstance<EnvironmentSpawnManager>();

        Debug.Log($"initialized input controller: {inputController}");
    }

    public void InitFollowCamera(FollowCamera cam)
    {
        FollowCamera = cam;
    }

    public void UpdateMovement(Vector2 swipeDelta)
    {
        Debug.Log($":: SwipeDelta: {swipeDelta}");
        if (isChangingLane) 
        {
            Debug.Log($":: isChangingLane early return");
            return;
        }
        
        if (swipeDelta.x > 0 && transform.position.x < rightBound.position.x)
        {
            MoveRight();
        }
        else if (swipeDelta.x < 0 && transform.position.x > leftBound.position.x)
        {
            MoveLeft();
        }
    }

    public void MoveLeft()
    {
        #if UNITY_EDITOR
        if (isChangingLane) return;
        #endif
        isChangingLane = true;
        targetPosition = transform.position + (Vector3.left * laneWidth);// + (Vector3.forward * 3f);

        transform.DOMove(targetPosition, laneChangeTime).OnComplete(() => 
        {
            isChangingLane = false;
        });
    }

    public void MoveRight()
    {
        #if UNITY_EDITOR
        if (isChangingLane) return;
        #endif

        isChangingLane = true;
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

        bogeyCollisionHandler = GetComponent<PlayerCollisionHandler>();
        laneWidth = (middleLane.position - rightLane.position).magnitude;
    }
}
