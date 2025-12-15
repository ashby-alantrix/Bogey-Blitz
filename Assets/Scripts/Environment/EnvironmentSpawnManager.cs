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

    private Queue<EnvironmentBlock> environmentBlocksQueue = new Queue<EnvironmentBlock>();
    private Transform passedEnvironmentBlock = null;

    public float EnvironmentMoveSpeed => environmentMoveSpeed;

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
        }
    }

    private void SendBlockTowardsEnd()
    {
        EnvironmentBlock dequeuedElement = environmentBlocksQueue.Dequeue();
        float offsetZ = environmentBlocksQueue.Last().transform.position.z + blockOffsetZ;

        dequeuedElement.transform.position = new Vector3(dequeuedElement.transform.position.x, dequeuedElement.transform.position.y, offsetZ);
        environmentBlocksQueue.Enqueue(dequeuedElement);
    }

    public void UpdateEnvBlockMoveSpeed(float moveSpeed)
    {
        foreach (EnvironmentBlock envBlock in environmentBlocksQueue)
        {
            envBlock.UpdateMoveSpeed(moveSpeed);
        }
    }
}
