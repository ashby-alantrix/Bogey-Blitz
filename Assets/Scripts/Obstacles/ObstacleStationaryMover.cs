using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleStationaryMover : MonoBehaviour
{
    private float moveSpeed = 30;

    public void InitMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * moveSpeed;
    }
}
