﻿using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [Tooltip("Cursor availability")]
    [SerializeField] private bool lockCursor = true;

    [Header("Camera")]
    [Tooltip("The sensitivity of the camera")]
    [SerializeField] private float sensitivity = 0.1f;
    [Tooltip("The virtual camera")]
    [SerializeField] private PlayerCharacterController playerCharacterController;

    [Space]
    [Header("Camera Rotation Constraints")]
    [Tooltip("The minimum angle of the camera")]
    [SerializeField] private float minimumAngle = -90f;
    [Tooltip("The maximum angle of the camera")]
    [SerializeField] private float maximumAngle = 90f;

    private float _xRotation;

    public Vector2 Look { get; set; }

    private void Awake()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void LateUpdate()
    {
        _xRotation -= Look.y * sensitivity;
        _xRotation = Mathf.Clamp(_xRotation, minimumAngle, maximumAngle);

        transform.localRotation = Quaternion.Euler(_xRotation, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);

        playerCharacterController.transform.Rotate(Look.x * sensitivity * Vector3.up);
    }
}