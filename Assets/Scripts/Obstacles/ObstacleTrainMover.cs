using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTrainMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * moveSpeed;
    }
}
