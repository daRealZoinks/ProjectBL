using Cinemachine;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 20f;
    [SerializeField] private float maxSpeed = 20f;

    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private Rigidbody _rigidbody;
    private float _xRotation;

    public Vector2 Direction { get; set; }
    public Vector2 Look { get; set; }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        var playerTransform = transform;
        var finalDirection = Direction.x * playerTransform.right + playerTransform.forward * Direction.y;

        if (finalDirection != Vector3.zero)
        {
            _rigidbody.AddForce(finalDirection.normalized * acceleration, ForceMode.Acceleration);
            _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, maxSpeed);
        }
        else
        {
            _rigidbody.AddForce(-_rigidbody.velocity * deceleration * Time.deltaTime, ForceMode.Acceleration);
        }

        _xRotation -= Look.y * sensitivity;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        cinemachineVirtualCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        _rigidbody.MoveRotation(transform.rotation * Quaternion.Euler(0f, Look.x * sensitivity, 0f));
    }
}