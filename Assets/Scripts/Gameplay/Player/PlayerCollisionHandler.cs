using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private Transform[] crashPoints;

    [SerializeField] private GameObject carModel;

    [SerializeField] private GameObject crashModelRef;
    [SerializeField] private float forceToApply = 750f;

    private Rigidbody[] meshParts;
    private Vector3[] savedMeshPositions;
    private PlayerCarController playerCarController;
    private GameManager gameManager;

    private void Awake()
    {
        InitializeCrashModelPositions();
    }

    public void InitializeCrashModelPositions()
    {
        int indexer = 0;
        savedMeshPositions = new Vector3[crashModelRef.transform.childCount];
        meshParts = new Rigidbody[crashModelRef.transform.childCount];

        foreach (Transform child in crashModelRef.transform)
        {
            savedMeshPositions[indexer] = child.position;
            meshParts[indexer] = child.GetComponent<Rigidbody>();
            indexer++;
        }
    }

    public void SetupNewCrashModel()
    {
        crashModelRef.gameObject.SetActive(false);
        for (int i=0; i< savedMeshPositions.Length; i++)
        {
            meshParts[i].velocity = Vector3.zero;
            meshParts[i].angularVelocity = Vector3.zero;
            meshParts[i].Sleep();
            meshParts[i].transform.position = savedMeshPositions[i];
        }
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
            case BogeyBlitz_Constants.Straight_Track_Tag:
                playerCarController.WorldSpawnManager.SetEnvironmentBlocks(other.transform);
            break;
            case BogeyBlitz_Constants.Obstacle_Tag:

                gameManager.OnGameStateChange(GameState.GameOver);

                ActivateCarModel(false);

                foreach (var meshPart in meshParts)
                {
                    int index = Random.Range(0, crashPoints.Length);
                    Vector3 dir = (crashPoints[index].position - transform.position).normalized;

                    meshPart.AddForce(forceToApply * dir, ForceMode.Force);
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
