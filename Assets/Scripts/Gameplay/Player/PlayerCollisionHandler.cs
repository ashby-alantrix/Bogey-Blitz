using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private Transform[] crashPoints;

    [SerializeField] private GameObject carModel;

    [SerializeField] private GameObject crashModelRef;

    private Rigidbody[] meshParts;
    private Vector3[] savedMeshPositions;
    private PlayerCarController playerCarController;
    private GameManager gameManager;

    private void Start()
    {
        InitializeCrashModelPositions();
    }

    public void InitializeCrashModelPositions()
    {
        int indexer = 0;
        savedMeshPositions = new Vector3[crashModelRef.transform.childCount];
        meshParts = new Rigidbody[crashModelRef.transform.childCount];

        Debug.Log($"crashModelRef.transform.childCount: {crashModelRef.transform.childCount}");

        foreach (Transform child in crashModelRef.transform)
        {
            savedMeshPositions[indexer] = child.position;
            meshParts[indexer] = child.GetComponent<Rigidbody>();
            indexer++;
        }
    }

    public void SetupNewCrashModel()
    {
        // int indexer = 0;
        // var crashModelInstance = Instantiate(crashModelRef, transform);
        crashModelRef.gameObject.SetActive(false);

        // crashModelInstance.transform.position = carModel.transform.position;

        // meshParts = new Rigidbody[crashModelRef.transform.childCount];

        // foreach (Transform part in crashModelRef.transform)
        // {
            
        // }

        for (int i=0; i< savedMeshPositions.Length; i++)
        {
            Debug.Log($"Setting new crash model: {savedMeshPositions[i]}");

            meshParts[i].position = savedMeshPositions[i];
        }

        Debug.Log($"Setting new crash model");
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckTriggerEvent(other);
    }

    private void CheckTriggerEvent(Collider other)
    {
        gameManager = gameManager == null ? InterfaceManager.Instance?.GetInterfaceInstance<GameManager>() : gameManager;
        playerCarController = playerCarController == null ? InterfaceManager.Instance?.GetInterfaceInstance<PlayerCarController>() : playerCarController;

        switch (other.tag)
        {
            case BogeyBlitz_Constants.STRAIGHT_TRACK_TAG:
                playerCarController.WorldSpawnManager.SetEnvironmentBlocks(other.transform);
            break;
            case BogeyBlitz_Constants.OBSTACLE_TAG:

                gameManager.OnGameStateChange(GameState.GameOver);

                ActivateCarModel(false);

                foreach (var meshPart in meshParts)
                {
                    int index = Random.Range(0, crashPoints.Length);
                    Vector3 dir = (crashPoints[index].position - transform.position).normalized;

                    meshPart.AddForce(1000f * dir);
                }
            break;
        }
    }

    public void ActivateCarModel(bool state)
    {
        carModel.SetActive(state);
        crashModelRef.SetActive(!state);
    }
}
