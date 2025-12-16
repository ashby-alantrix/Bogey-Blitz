using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableMoverBase : MonoBehaviour
{
    private float moveSpeed;

    public void InitMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    protected void Update()
    {
        transform.position += transform.forward * Time.deltaTime * moveSpeed;
    }
}
