using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnvironmentBlock : MonoBehaviour
{
    [SerializeField] private Transform startpoint;
    [SerializeField] private Transform endpoint;

    private float moveSpeed;

    private WorldSpawnManager worldSpawnManager;

    public Vector3 Startpoint => startpoint.position;
    public Vector3 Endpoint => endpoint.position;

    public int ID
    {
        get;
        private set;
    }

    public void Init(int id, float moveSpeed)
    {
        this.ID = id;
        UpdateMoveSpeed(moveSpeed);
    }

    public void UpdateMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    private void Update()
    {
        if (worldSpawnManager)
        {
            transform.position += -Vector3.forward * Time.deltaTime * worldSpawnManager.EnvironmentMoveSpeed;
            Debug.Log($"moving blocks: {transform.position}");
        }
        else
            worldSpawnManager = InterfaceManager.Instance?.GetInterfaceInstance<WorldSpawnManager>();
    }
}
