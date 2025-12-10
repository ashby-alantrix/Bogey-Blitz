using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentBlock : MonoBehaviour
{
    private float moveSpeed;

    public int ID
    {
        get;
        private set;
    }

    public void Init(int id, float moveSpeed)
    {
        this.ID = id;
        this.moveSpeed = moveSpeed;
    }

    void Update()
    {
        transform.position += -Vector3.forward * Time.deltaTime * moveSpeed;
    }
}
