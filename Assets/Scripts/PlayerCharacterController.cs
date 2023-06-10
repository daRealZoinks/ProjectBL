using Cinemachine;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float acceleration = 5120; // m/s^2
    [SerializeField] private float deceleration = 800f; // m/s^2
    [SerializeField] private float maxSpeed = 15; // m/s

    [Space]
    [Header("Jump")]
    [SerializeField] private float maxAirSpeed = 20f; // m/s
    [SerializeField] private float jumpHeight = 3f; // m
    [SerializeField] private float airControl = 0.05f; // 0-1 (0 = no control, 1 = full control)
    [SerializeField] private float airBrake = 0f; // 0-1 (0 = no brake, 1 = full brake)
    [SerializeField] private LayerMask groundLayerMask;

    [Space]
    [Header("Camera")]
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private Rigidbody _rigidbody;
    private float _xRotation;
    private bool _grounded;

    public Vector2 Direction { get; set; }
    public Vector2 Look { get; set; }
    public bool Jumping { get; set; }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionStay(Collision other)
    {
        if (groundLayerMask == (groundLayerMask | (1 << other.gameObject.layer)))
        {
            _grounded = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (groundLayerMask == (groundLayerMask | (1 << other.gameObject.layer)))
        {
            _grounded = false;
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Jumping && _grounded)
        {
            Jump();
        }

        _xRotation -= Look.y * sensitivity;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        cinemachineVirtualCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        _rigidbody.MoveRotation(transform.rotation * Quaternion.Euler(0f, Look.x * sensitivity, 0f));
    }

    private void FixedUpdate()
    {
        var velocity = _rigidbody.velocity;
        var horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

        var playerTransform = transform;
        var finalDirection = Direction.x * playerTransform.right + playerTransform.forward * Direction.y;

        var finalForce = finalDirection != Vector3.zero ? finalDirection * acceleration : -horizontalVelocity * deceleration;

        finalForce *= finalDirection != Vector3.zero ? _grounded ? 1f : airControl : _grounded ? 1f : airBrake;

        // we should do this in another way
        if (_grounded)
        {
            // we will calculate the direction of the horizontal velocity
            // then we will subtract it from the final force so that we don't exceed the max speed

            if (horizontalVelocity.magnitude > maxSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
                _rigidbody.velocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);
            }
        }

        _rigidbody.AddForce(finalForce * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    private void Jump()
    {
        var jumpForce = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        Jumping = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var playerTransform = transform;
        Gizmos.DrawRay(playerTransform.position, Direction.x * playerTransform.right + Direction.y * playerTransform.forward);
    }
}