using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableMoverBase : MonoBehaviour
{
    protected float moveSpeed;
    protected WorldSpawnManager worldSpawnManager;

    protected virtual void Update()
    {
        transform.position += transform.forward * Time.deltaTime * moveSpeed;
    }
}
