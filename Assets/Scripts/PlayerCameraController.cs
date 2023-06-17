using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [Tooltip("The sensitivity of the camera")]
    [SerializeField] private float sensitivity = 0.1f;
    [Tooltip("The virtual camera")]
    [SerializeField] private PlayerCharacterController playerCharacterController;
    [Space]
    [Tooltip("The minimum x rotation of the camera")]
    [SerializeField] private float minXRotation = -90;
    [Tooltip("The maximum x rotation of the camera")]
    [SerializeField] private float maxXRotation = 90;
    
    private float _xRotation;

    public Vector2 Look { get; set; }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        _xRotation -= Look.y * sensitivity;
        _xRotation = Mathf.Clamp(_xRotation, minXRotation, maxXRotation);
        
        transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        playerCharacterController.transform.Rotate(Look.x * sensitivity * Vector3.up);
    }
}