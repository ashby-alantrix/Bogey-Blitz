using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCarController : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private Transform startPoint;
    [Header("Distance attributes")]
    [SerializeField] private float totalDistanceToCover = 10000;

    [Header("Move speed attributes")]
    [SerializeField] private float baseSpeed = 15;
    [SerializeField] private float maxSpeed = 30;

    [SerializeField] private float laneWidth;
    [SerializeField] private float laneChangeTime;
    [SerializeField] private float laneChangeSpeed;

    [SerializeField] private Transform leftBound;
    [SerializeField] private Transform rightBound;  
    [SerializeField] private Transform middleLane;
    [SerializeField] private Transform rightLane;

    [SerializeField] private Vector3 dir;

    private bool isChangingLane = false;
    private float distanceToRailEndpoint;
    private float timeToRailEndpoint;
    private float distTimer = 0;
    private float distanceCovered = 0;
    private float lastCoveredDistance = 0;

    private Vector3 targetPosition = Vector3.zero;
    private AnimationCurve worldMoveSpeed;

    public GameManager GameManager { get; private set; }
    private InputController inputController;
    private WorldSpawnManager worldSpawnManager;
    private DifficultyEvaluator difficultyEvaluator;
    private PlayerCollisionHandler playerCollisionHandler;
    
    public PlayerCollisionHandler PlayerCollisionHandler => playerCollisionHandler;
    public FollowCamera FollowCamera
    {
        get;
        private set;
    }

    public float CurrentEvaluatedSpeed01
    {
        get; 
        private set;        
    }
    
    public float CurrentCoveredDistance01 => distanceCovered / totalDistanceToCover;
    public float CurrentCoveredDistance => distanceCovered;

    public WorldSpawnManager WorldSpawnManager => worldSpawnManager;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<PlayerCarController>(this);
    }

    public void InitializeData()
    {
        inputController = InterfaceManager.Instance?.GetInterfaceInstance<InputController>();
        difficultyEvaluator  = InterfaceManager.Instance?.GetInterfaceInstance<DifficultyEvaluator>();
        SetGameManager();
        SetWorldSpawnManager();

        worldMoveSpeed = difficultyEvaluator.DifficultyCurveSO.GetDifficultyCurve(DifficultyCurveType.WorldMoveSpeed);

        Debug.Log($"initialized input controller: {inputController}");

        OnDataReset();

        Debug.Log($":: DISTANCE TO COVER :: {distanceToRailEndpoint}");
    }

    public void OnDataReset()
    {
        distTimer = 0;
        distanceToRailEndpoint = worldSpawnManager.GetFirstEnvironmentBlock().Endpoint.z - transform.position.z;
        timeToRailEndpoint = distanceToRailEndpoint / worldSpawnManager.EnvironmentMoveSpeed;
    }

    public void SetBaseMovementSpeed()
    {
        SetWorldSpawnManager();
        worldSpawnManager.SetEnvironmentMoveSpeed(baseSpeed);
    }

    public void ResetEnvironmentBaseSpeed()
    {
        SetWorldSpawnManager();
        worldSpawnManager.SetEnvironmentMoveSpeed(0);
    }

    private void SetGameManager()
    {
        GameManager  = InterfaceManager.Instance?.GetInterfaceInstance<GameManager>();
    }

    private void SetWorldSpawnManager()
    {
        worldSpawnManager = worldSpawnManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<WorldSpawnManager>() : worldSpawnManager;
    }

    public void ResetData()
    {
        distanceCovered = lastCoveredDistance = 0f;

        ResetEnvironmentBaseSpeed();
        transform.position = startPoint.position;
    }

    public void InitFollowCamera(FollowCamera cam)
    {
        FollowCamera = cam;
    }

    public void UpdateMovement(Vector2 swipeDelta)
    {
        Debug.Log($":: SwipeDelta: {swipeDelta.magnitude}");
        if (isChangingLane) 
        {
            Debug.Log($":: isChangingLane early return");
            return;
        }
        
        GameManager.SoundManager.PlayPrimaryGameSoundClip(SoundType.CarGearChange);
        if (swipeDelta.x > 1f && transform.position.x < rightBound.position.x)
        {
            MoveRight();
        }
        else if (swipeDelta.x < -1f && transform.position.x > leftBound.position.x)
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
        
        #if UNITY_EDITOR
        GameManager.SoundManager.PlayPrimaryGameSoundClip(SoundType.CarGearChange);
        #endif

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

        #if UNITY_EDITOR
        GameManager.SoundManager.PlayPrimaryGameSoundClip(SoundType.CarGearChange);
        #endif

        transform.DOMove(targetPosition, laneChangeTime).OnComplete(() => 
        {
            isChangingLane = false;
        });

        Debug.Log($"Target position: {targetPosition}");
    }

    private void Awake()
    {
        playerCollisionHandler = GetComponent<PlayerCollisionHandler>();
    }

    private void Start()
    {
        dir = Vector3.forward;    

        laneWidth = (middleLane.position - rightLane.position).magnitude;
    }

    private void Update()
    {
        if (!GameManager || !GameManager.IsGameInProgress) return;

        Debug.Log($"#### PlayerCarController Update");

        if (distanceCovered > 0)
        {
            Debug.Log($"#### Has distance covered: {distanceCovered}");
            GameManager.InGameUIManager.UpdateHUDDistance($"{Mathf.FloorToInt(distanceCovered)}");

            CurrentEvaluatedSpeed01 = worldMoveSpeed.Evaluate(distanceCovered / totalDistanceToCover);
            worldSpawnManager.SetEnvironmentMoveSpeed(
                worldSpawnManager.GetResultBasedOnDifficultyProgressiveFormula(
                    startVal: baseSpeed, 
                    endVal: maxSpeed, 
                    fraction: CurrentEvaluatedSpeed01));
        }

        if (distTimer < timeToRailEndpoint)
        {
            distTimer += Time.deltaTime;
            distanceCovered = lastCoveredDistance + (worldSpawnManager.EnvironmentMoveSpeed * distTimer);

            Debug.Log($"DistanceCovered: {distanceCovered}");
            Debug.Log($"DistanceCovered, lastCoveredDistance: {lastCoveredDistance}");
            Debug.Log($"DistanceCovered, EnvironmentMoveSpeed: {worldSpawnManager.EnvironmentMoveSpeed}");
        }
        else
        {
            distTimer = 0;
            timeToRailEndpoint = 0;
            lastCoveredDistance = distanceCovered;

            if (worldSpawnManager.GetNewlyEncounteredEnvironmentBlock() != null)
            {
                distanceToRailEndpoint = worldSpawnManager.GetNewlyEncounteredEnvironmentBlock().Endpoint.z - transform.position.z;
                timeToRailEndpoint = distanceToRailEndpoint / worldSpawnManager.EnvironmentMoveSpeed;

                worldSpawnManager.ResetNewlyEncounteredEnvironmentBlock();
            }
        }   

        Debug.Log($"#### PlayerCarController Update: {distanceCovered}");
    }
}
