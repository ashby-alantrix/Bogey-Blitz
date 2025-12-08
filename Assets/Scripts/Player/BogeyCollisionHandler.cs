using UnityEngine;

public class BogeyCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CheckBlockEvent(other.tag);
    }

    private void CheckBlockEvent(string tag)
    {
        switch (tag)
        {
            case BogeyBlitz_Constants.STRAIGHT_TRACK_TAG:
                
            break;
            case BogeyBlitz_Constants.CURVED_TRACK_TAG:

            break;
        }
    }
}
