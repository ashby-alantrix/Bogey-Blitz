using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class EnvironmentSpawnManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private Transform planesParent;
    [SerializeField] private GameObject testPrefab;
    [SerializeField] private int testPrefabCount;

    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSpawnCount = 14;
    [SerializeField] private float blockOffsetZ = 15;

    private BogeyController bogeyController;
    private Queue<Transform> environmentBlocksQueue = new Queue<Transform>();

    private Transform prevEnvironmentBlock = null;
    private Transform currentEnvironmentBlock = null;

    private float lastSavedZOffset;

    [ContextMenu("Create Test Blocks")]
    public void CreateTestBlocks()
    {
        float zOffsetSum = 0;
        for (int i=0; i<testPrefabCount; i++)
        {
            zOffsetSum += blockOffsetZ;
            var instance = Instantiate(testPrefab, new Vector3(testPrefab.transform.position.x, testPrefab.transform.position.y, zOffsetSum), Quaternion.identity);
            instance.transform.SetParent(planesParent);
            instance.name = $"Plane {i + 1}";
        }
    }

    private void Awake()
    {
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

    public void SetEnvironmentBlocks(Transform block)
    {
        Debug.Log($"SetEnvironmentBlocks :: before prevEnvironmentBlock: {prevEnvironmentBlock}");
        Debug.Log($"SetEnvironmentBlocks :: before currentEnvironmentBlock: {currentEnvironmentBlock}");

        prevEnvironmentBlock = currentEnvironmentBlock;
        currentEnvironmentBlock = block;

        Debug.Log($"SetEnvironmentBlocks :: after prevEnvironmentBlock: {prevEnvironmentBlock}");
        Debug.Log($"SetEnvironmentBlocks :: after currentEnvironmentBlock: {currentEnvironmentBlock}");

        if (prevEnvironmentBlock != null)
        {
            lastSavedZOffset = bogeyController.transform.position.z - currentEnvironmentBlock.transform.position.z;
            Debug.Log($":: lastSavedZOffset: currentEnvironmentBlock: {currentEnvironmentBlock.name}");
            Debug.Log($":: lastSavedZOffset. bogeyController.transform.position.z : {bogeyController.transform.position.z }");
            Debug.Log($":: lastSavedZOffset. currentEnvironmentBlock.transform.position.z: {currentEnvironmentBlock.transform.position.z}");
            Debug.Log($":: lastSavedZOffset: {lastSavedZOffset}");
            SendBlockTowardsEnd();
        }
    }

    // OnTrigger
    private void SendBlockTowardsEnd()
    {
        var dequeuedElement = environmentBlocksQueue.Dequeue();
        var offsetZ = environmentBlocksQueue.Last().transform.position.z + blockOffsetZ;

        dequeuedElement.transform.position = new Vector3(dequeuedElement.transform.position.x, dequeuedElement.transform.position.y, offsetZ);
        environmentBlocksQueue.Enqueue(dequeuedElement);
    }

    private void Update()
    {
        if (!bogeyController) return;

        if (bogeyController.transform.position.z > 202.5f)
        {
            lastSavedZOffset = bogeyController.transform.position.z - currentEnvironmentBlock.transform.position.z;
            Debug.Log($"Resetting world origin: {lastSavedZOffset}");
            bogeyController.transform.position = new Vector3(bogeyController.transform.position.x, bogeyController.transform.position.y, Mathf.Abs(lastSavedZOffset));

            float blockPosZ = 0;

            foreach (var block in environmentBlocksQueue)
            {
                block.position = new Vector3(block.position.x, block.position.y, blockPosZ);
                blockPosZ += blockOffsetZ;
            }
        }
    }
}
