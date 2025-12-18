using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class WorldSpawnManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private Transform planesParent;
    [SerializeField] private GameObject testPrefab;
    [SerializeField] private int testPrefabCount;

    [SerializeField] private GameObject prefab;
    [SerializeField] private float blockOffsetZ = 5;
    [SerializeField] private float environmentMoveSpeed;
    [SerializeField] private float queueEnqueueDelay = 1f;

    private float lastZOffset;

    private Queue<EnvironmentBlock> environmentBlocksQueue = new Queue<EnvironmentBlock>();
    private Transform passedEnvironmentBlock = null;

    private EnvironmentBlock passedEnvironmentBlockComp = null;
    private EnvironmentBlock newlyEncounteredEnvironmentBlock = null;
    private EnvironmentBlock firstEnvironmentBlock = null;
    private PlayerCarController playerCarController = null;

    public float EnvironmentMoveSpeed => environmentMoveSpeed;

    public void SetEnvironmentMoveSpeed(float newSpeedVal)
    {
        Debug.Log($"newSpeedVal: {newSpeedVal}");
        environmentMoveSpeed = newSpeedVal;
        UpdateEnvBlockMoveSpeed(newSpeedVal);
    }

    public EnvironmentBlock GetFirstEnvironmentBlock()
    {
        return firstEnvironmentBlock;
    }

    public EnvironmentBlock GetNewlyEncounteredEnvironmentBlock()
    {
        return newlyEncounteredEnvironmentBlock;
    }

    public void ResetNewlyEncounteredEnvironmentBlock()
    {
        newlyEncounteredEnvironmentBlock = null;
    }

    [ContextMenu("Create Test Blocks")]
    public void CreateBlocks()
    {
        float zOffsetSum = 0;
        for (int i=0; i<testPrefabCount; i++)
        {
            GameObject instance = Instantiate(testPrefab, new Vector3(testPrefab.transform.position.x, testPrefab.transform.position.y, zOffsetSum), Quaternion.identity);
            instance.transform.SetParent(planesParent);
            instance.name = $"Rail {i + 1}";

            EnvironmentBlock block = instance.GetComponent<EnvironmentBlock>();
            block.Init(i + 1, EnvironmentMoveSpeed);

            if (firstEnvironmentBlock == null)
                firstEnvironmentBlock = block;

            zOffsetSum += blockOffsetZ;
        }
    }

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<WorldSpawnManager>(this);
    }

    public void InitializeData()
    {
        playerCarController = InterfaceManager.Instance?.GetInterfaceInstance<PlayerCarController>();
    }

    public void SetEnvironmentBlocks(Transform newBlock)
    {
        passedEnvironmentBlock = newBlock;
        if (passedEnvironmentBlock != null)
        {
            SendBlockTowardsEnd();
            InitNewEnvironmentBlock();
        }
    }

    public void UpdateEnvBlockMoveSpeed(float moveSpeed)
    {
        foreach (EnvironmentBlock envBlock in environmentBlocksQueue)
        {
            envBlock.UpdateMoveSpeed(moveSpeed);
        }
    }

    private void SendBlockTowardsEnd()
    {
        passedEnvironmentBlockComp = environmentBlocksQueue.Dequeue();
        Invoke(nameof(UpdatePositionForDequeuedElement), queueEnqueueDelay);
    }

    private void UpdatePositionForDequeuedElement()
    {
        lastZOffset = environmentBlocksQueue.Last().transform.position.z + blockOffsetZ;
        passedEnvironmentBlockComp.transform.position = new Vector3(passedEnvironmentBlockComp.transform.position.x, passedEnvironmentBlockComp.transform.position.y, lastZOffset);

        environmentBlocksQueue.Enqueue(passedEnvironmentBlockComp);
    }

    private void InitNewEnvironmentBlock()
    {
        newlyEncounteredEnvironmentBlock = environmentBlocksQueue.Peek();
        Debug.Log($":: newlyEncounteredEnvironmentBlock: {newlyEncounteredEnvironmentBlock.name}");
    }

    private void Awake()
    {
        CreateBlocks();

        foreach (Transform transformObj in planesParent)
        {
            environmentBlocksQueue.Enqueue(transformObj.GetComponent<EnvironmentBlock>());
        }
    }
}
