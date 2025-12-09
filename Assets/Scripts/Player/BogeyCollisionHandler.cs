using UnityEngine;

public class BogeyCollisionHandler : MonoBehaviour
{
    private BogeyController bogeyController;
    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        bogeyController = GetComponent<BogeyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Checking block event: {other.tag}");
        CheckBlockEvent(other);
    }

    private void CheckBlockEvent(Collider other)
    {
        switch (other.tag)
        {
            case BogeyBlitz_Constants.STRAIGHT_TRACK_TAG:
                bogeyController.EnvironmentSpawnManager.SetEnvironmentBlocks(other.transform);
            break;
            case BogeyBlitz_Constants.CURVED_TRACK_TAG:

            break;
        }
    }

    public void ToggleColliderState(bool state)
    {
        boxCollider.enabled = state;
    }
}
