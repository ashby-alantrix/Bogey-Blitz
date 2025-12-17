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

    void Update()
    {
        transform.position += -Vector3.forward * Time.deltaTime * moveSpeed;
    }
}
