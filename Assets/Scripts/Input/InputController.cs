using UnityEngine;
using UnityEngine.EventSystems;
// using UnityEngine.InputSystem.EnhancedTouch;

public class InputController : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    // Finger finger;
    [SerializeField] private bool enableKeyboardControl;

    private float laneWidth;

    private Vector3 startTouchPosition = Vector3.zero;
    private Vector3 currentTouchPosition = Vector3.zero;
    private Vector3 targetPosition = Vector3.zero;
    private bool isSwiping = false;
    private bool canChangeLane = false;

    private PlayerCarController carController;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<InputController>(this);
    }

    public void InitializeData()
    {
        carController = InterfaceManager.Instance?.GetInterfaceInstance<PlayerCarController>();
        Debug.Log($"initialized bogey controller: {carController}");
    }

    private void Update()
    {
        if (!carController || !carController?.GameManager || !carController.GameManager.IsGameInProgress) return;

#if UNITY_EDITOR
        if (enableKeyboardControl)
        {
            if (Input.GetKeyDown(KeyCode.A))
                carController.MoveLeft();
            else if (Input.GetKeyDown(KeyCode.D))
                carController.MoveRight();
        }
#endif

        DetectSwipe();
    }

    private void DetectSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    isSwiping = true;
                    break;
                case TouchPhase.Moved:
                    currentTouchPosition = touch.position;
                    break;
                case TouchPhase.Ended:
                    if (isSwiping)
                    {
                        isSwiping = false;
                        Vector2 swipeDelta = currentTouchPosition - startTouchPosition;
                        Debug.Log($"swipeDelta.x: {swipeDelta.x}");
                        carController.UpdateMovement(swipeDelta);
                    }
                    break;
            }
        }
    }
}
