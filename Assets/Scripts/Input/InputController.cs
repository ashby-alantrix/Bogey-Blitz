using UnityEngine;
// using UnityEngine.InputSystem.EnhancedTouch;

public class InputController : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private Transform leftBound;
    [SerializeField] private Transform rightBound;

    // Finger finger;

    private float laneWidth;

    private Vector3 startTouchPosition = Vector3.zero;
    private Vector3 currentTouchPosition = Vector3.zero;
    private Vector3 targetPosition = Vector3.zero;
    private bool isSwiping = false;
    private bool canChangeLane = false;

    private BogeyController bogeyController;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<InputController>(this);
    }

    public void InitializeData()
    {
        bogeyController = InterfaceManager.Instance?.GetInterfaceInstance<BogeyController>();
        Debug.Log($"initialized bogey controller: {bogeyController}");
    }

    private void Update()
    {
        if (!bogeyController) return;

        DetectSwipe();
    }

    private void DetectSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
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
                        Debug.Log($"transform.position.x < rightBound.position.x: {transform.position.x} :: {rightBound.position.x}");

                        if (swipeDelta.x > 0 && transform.position.x < rightBound.position.x)
                        {
                            bogeyController.MoveRight();
                        }
                        else if (swipeDelta.x < 0 && transform.position.x > leftBound.position.x)
                        {   
                            bogeyController.MoveLeft();
                        }
                    }
                    break;
            }
        }
    }
}
