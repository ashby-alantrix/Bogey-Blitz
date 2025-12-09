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
    [SerializeField] private int initialSpawnCount = 14;
    [SerializeField] private float blockOffsetZ = 5;

    private BogeyController bogeyController;
    private Queue<Transform> environmentBlocksQueue = new Queue<Transform>();
    private Dictionary<string, EnvironmentBlock> environmentBlocksDict = new Dictionary<string, EnvironmentBlock>();

    private Transform prevEnvironmentBlock = null;
    private Transform currentEnvironmentBlock = null;

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
            block.Init(i + 1);

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
        Debug.Log($"SetEnvironmentBlocks :: before prevEnvironmentBlock: {prevEnvironmentBlock?.name}");
        Debug.Log($"SetEnvironmentBlocks :: before currentEnvironmentBlock: {currentEnvironmentBlock?.name}");

        if (currentEnvironmentBlock != null && environmentBlocksDict[newBlock.name].ID != environmentBlocksDict[currentEnvironmentBlock.name].ID + 1 || 
            environmentBlocksDict[newBlock.name].ID == 1 && environmentBlocksDict[currentEnvironmentBlock.name].ID == environmentBlocksDict.Count)
        {
            return;
        }

        // if (!string.IsNullOrWhiteSpace(prevEnvironmentBlock?.name) && !string.IsNullOrWhiteSpace(currentEnvironmentBlock?.name) 
        //     &&  (prevEnvironmentBlock.name.Equals(currentEnvironmentBlock.name) || currentEnvironmentBlock.name.Equals(newBlock.name))) 
        // {
        //     return;
        // }

        prevEnvironmentBlock = currentEnvironmentBlock;
        currentEnvironmentBlock = newBlock;

        Debug.Log($"SetEnvironmentBlocks :: after prevEnvironmentBlock: {prevEnvironmentBlock?.name}");
        Debug.Log($"SetEnvironmentBlocks :: after currentEnvironmentBlock: {currentEnvironmentBlock.name}");

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

    private void SendBlockTowardsEnd()
    {
        Debug.Log($"SetEnvironmentBlocks :: after :: SendBlockTowardsEnd");
        var dequeuedElement = environmentBlocksQueue.Dequeue();
        var offsetZ = environmentBlocksQueue.Last().transform.position.z + blockOffsetZ;

        dequeuedElement.transform.position = new Vector3(dequeuedElement.transform.position.x, dequeuedElement.transform.position.y, offsetZ);
        environmentBlocksQueue.Enqueue(dequeuedElement);
    }


    private void Update()
    {
        if (!bogeyController) return;

        if (bogeyController.transform.position.z > 110f)
        {
            bogeyController.BogeyCollisionHandler.ToggleColliderState(false);
            float blockPosZ = 0;

            foreach (var block in environmentBlocksQueue)
            {
                block.position = new Vector3(block.position.x, block.position.y, blockPosZ);
                blockPosZ += blockOffsetZ;
            }

            bogeyController.transform.position = new Vector3(bogeyController.transform.position.x, bogeyController.transform.position.y, Mathf.Abs(lastSavedZOffset));
            bogeyController.BogeyCollisionHandler.ToggleColliderState(true);
            Debug.Log($"Resetting world origin: {lastSavedZOffset}");
        }
    }
}
