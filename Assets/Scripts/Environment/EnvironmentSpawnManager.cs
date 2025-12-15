using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class EnvironmentSpawnManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private Transform planesParent;
    [SerializeField] private GameObject testPrefab;
    [SerializeField] private int testPrefabCount;

    [SerializeField] private GameObject prefab;
    [SerializeField] private float blockOffsetZ = 5;
    [SerializeField] private float environmentMoveSpeed;

    private BogeyController bogeyController;
    private Queue<Transform> environmentBlocksQueue = new Queue<Transform>();
    private Dictionary<string, EnvironmentBlock> environmentBlocksDict = new Dictionary<string, EnvironmentBlock>();

    private Transform prevEnvironmentBlock = null;
    private Transform passedEnvironmentBlock = null;

    private float lastSavedZOffset;

    [ContextMenu("Create Test Blocks")]
    public void CreateTestBlocks()
    {
        float zOffsetSum = 0;
        for (int i=0; i<testPrefabCount; i++)
        {
            var instance = Instantiate(testPrefab, new Vector3(testPrefab.transform.position.x, testPrefab.transform.position.y, zOffsetSum), Quaternion.identity);
            instance.transform.SetParent(planesParent);
            instance.name = $"Rail {i + 1}";

            var block = instance.GetComponent<EnvironmentBlock>();
            environmentBlocksDict.Add(instance.name, block);
            block.Init(i + 1, environmentMoveSpeed);

            zOffsetSum += blockOffsetZ;
        }
    }

    private void Awake()
    {
        CreateTestBlocks();

        foreach (Transform transformObj in planesParent)
        {
            environmentBlocksQueue.Enqueue(transformObj);
        }
    }

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<EnvironmentSpawnManager>(this);
    }

    public void InitializeData()
    {
        bogeyController = InterfaceManager.Instance?.GetInterfaceInstance<BogeyController>();
    }

    public void SetEnvironmentBlocks(Transform newBlock)
    {
        prevEnvironmentBlock = passedEnvironmentBlock;
        passedEnvironmentBlock = newBlock;

        if (passedEnvironmentBlock != null)
        {
            SendBlockTowardsEnd();
        }
    }

    private void SendBlockTowardsEnd()
    {
        var dequeuedElement = environmentBlocksQueue.Dequeue();
        var offsetZ = environmentBlocksQueue.Last().transform.position.z + blockOffsetZ;

        dequeuedElement.transform.position = new Vector3(dequeuedElement.transform.position.x, dequeuedElement.transform.position.y, offsetZ);
        environmentBlocksQueue.Enqueue(dequeuedElement);
    }
}
