using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Vector3 camOffset;
    [SerializeField] private PlayerCarController playerCarController;

    private void Start()
    {
        playerCarController.InitFollowCamera(this);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.position = playerCarController.transform.position + camOffset;
    }

    public void SetCamState(bool state)
    {
        enabled = state;
    }
}
