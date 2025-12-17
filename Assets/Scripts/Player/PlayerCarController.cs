using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCarController : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private bool shouldStop;
    [SerializeField] private Transform leftBound;
    [SerializeField] private Transform rightBound;

    [SerializeField] private Vector3 dir;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float laneWidth;
    [SerializeField] private float laneChangeTime;
    [SerializeField] private float laneChangeSpeed;

    [SerializeField] private Transform middleLane;
    [SerializeField] private Transform rightLane;

    private bool isChangingLane = false;
    private float distanceToRailEndpoint;
    private float timeTowardRailEndpoint;
    private float distTimer = 0;
    private float distanceCovered = 0;
    private float prevRailDistanceCovered = 0;

    private Vector3 targetPosition = Vector3.zero;
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

        distTimer = 0;
        distanceToRailEndpoint = environmentSpawnManager.GetFirstEnvironmentBlock().Endpoint.z - transform.position.z;
        timeTowardRailEndpoint = distanceToRailEndpoint / environmentSpawnManager.EnvironmentMoveSpeed;

        Debug.Log($":: DISTANCE TO COVER :: {distanceToRailEndpoint}");
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

    private void Update()
    {
        if (distTimer < timeTowardRailEndpoint)
        {
            distTimer += Time.deltaTime;
            // distanceCovered = prevRailDistanceCovered + environmentSpawnManager.EnvironmentMoveSpeed * distTimer;
            distanceCovered = environmentSpawnManager.EnvironmentMoveSpeed * distTimer;
            Debug.Log($":: DISTANCE COVERED :: {distanceCovered}");
        }
        else
        {
            distTimer = 0;
            timeTowardRailEndpoint = 0;
            prevRailDistanceCovered = distanceCovered;

            if (environmentSpawnManager.GetNewlyEncounteredEnvironmentBlock() != null)
            {
                distanceToRailEndpoint = environmentSpawnManager.GetNewlyEncounteredEnvironmentBlock().Endpoint.z - transform.position.z;
                timeTowardRailEndpoint = distanceToRailEndpoint / environmentSpawnManager.EnvironmentMoveSpeed;

                Debug.Log($"rail points :: startpoint position: {environmentSpawnManager.GetNewlyEncounteredEnvironmentBlock().Startpoint}");
                Debug.Log($"rail points :: endpoint position: {environmentSpawnManager.GetNewlyEncounteredEnvironmentBlock().Endpoint}");
                Debug.Log($"rail points :: distance between rail points: {(environmentSpawnManager.GetNewlyEncounteredEnvironmentBlock().Endpoint - environmentSpawnManager.GetNewlyEncounteredEnvironmentBlock().Startpoint).magnitude}");
                Debug.Log($"rail points :: distance between player and rail point: {(environmentSpawnManager.GetNewlyEncounteredEnvironmentBlock().Endpoint - transform.position).magnitude}");
                Debug.Log($"rail points :: distanceToRailEndpoint: {distanceToRailEndpoint}, {environmentSpawnManager.GetNewlyEncounteredEnvironmentBlock().name}");
                environmentSpawnManager.ResetNewlyEncounteredEnvironmentBlock();
                Debug.Log($"rail points :: timeTowardRailEndpoint: {timeTowardRailEndpoint}");
            }
            else
            {
                Debug.Log($"environmentSpawnManager.GetNewlyEncounteredEnvironmentBlock(): {environmentSpawnManager.GetNewlyEncounteredEnvironmentBlock()?.name}");
            }
        }   

        if (transform.position.z > environmentSpawnManager.GetFirstEnvironmentBlock().Endpoint.z && shouldStop)
        {
            shouldStop = false;
            Debug.Break();
        }
    }
}
