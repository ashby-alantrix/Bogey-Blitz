using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Vector3 camOffset;
    [SerializeField] private BogeyController bogeyController;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = bogeyController.transform.position + camOffset;
    }
}
