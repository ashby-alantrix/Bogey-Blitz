using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private Transform[] crashPoints;

    [SerializeField] private Rigidbody[] meshParts;
    [SerializeField] private GameObject carModel;
    [SerializeField] private GameObject crashModel;

    private PlayerCarController playerCarController;
    private GameManager gameManager;
    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckTriggerEvent(other);
    }

    private void CheckTriggerEvent(Collider other)
    {
        gameManager = gameManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<GameManager>() : gameManager;
        playerCarController = playerCarController == null ? InterfaceManager.Instance?.GetInterfaceInstance<PlayerCarController>() : playerCarController;

        Debug.Log($":: other.tag: {other.tag}");
        switch (other.tag)
        {
            case BogeyBlitz_Constants.STRAIGHT_TRACK_TAG:
                playerCarController.EnvironmentSpawnManager.SetEnvironmentBlocks(other.transform);
            break;
            case BogeyBlitz_Constants.OBSTACLE_TAG:

                gameManager.OnGameOver();

                carModel.SetActive(false);
                crashModel.SetActive(true);

                Debug.Log($"Activating crash model");
                foreach (var meshPart in meshParts)
                {
                    int index = Random.Range(0, crashPoints.Length);
                    Vector3 dir = (crashPoints[index].position - transform.position).normalized;
                    meshPart.AddForce(1000f * dir);
                }
            break;
        }
    }
}
