using System;
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
    [SerializeField] private float queueEnqueueDelay = 1f;

    private Queue<EnvironmentBlock> environmentBlocksQueue = new Queue<EnvironmentBlock>();
    private Transform passedEnvironmentBlock = null;

    private EnvironmentBlock passedEnvironmentBlockComp = null;
    private EnvironmentBlock newlyEncounteredEnvironmentBlock = null;
    private EnvironmentBlock firstEnvironmentBlock = null;

    public float EnvironmentMoveSpeed => environmentMoveSpeed;

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

    private void Awake()
    {
        CreateBlocks();

        foreach (Transform transformObj in planesParent)
        {
            environmentBlocksQueue.Enqueue(transformObj.GetComponent<EnvironmentBlock>());
        }
    }

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<EnvironmentSpawnManager>(this);
    }

    public void InitializeData()
    {
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

    private float lastZOffset;

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
    }

    public void UpdateEnvBlockMoveSpeed(float moveSpeed)
    {
        foreach (EnvironmentBlock envBlock in environmentBlocksQueue)
        {
            envBlock.UpdateMoveSpeed(moveSpeed);
        }
    }
}
