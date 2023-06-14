using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCharacterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float acceleration = 5120; // m/s^2
    [SerializeField] private float deceleration = 800f; // m/s^2
    [SerializeField] private float maxSpeed = 15; // m/s

    [Space]
    [Header("Jump")]
    [SerializeField] private float jumpHeight = 3f; // m
    [SerializeField] private float airControl = 0.05f; // 0-1 (0 = no control, 1 = full control)
    [SerializeField] private float airBrake; // 0-1 (0 = no brake, 1 = full brake)
    [SerializeField] private float groundCheckDistance = 1.5f; // m
    [SerializeField] private LayerMask groundLayerMask;

    [Space]
    [Header("Camera")]
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private Rigidbody _rigidbody;
    private float _xRotation;
    private bool _grounded;

    public Vector2 Direction { get; set; }
    public Vector2 Look { get; set; }
    public bool Jumping { get; set; }
    public bool Grounded
    {
        get => _grounded;
        set
        {
            _grounded = value;
        }
    }

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

        virtualCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        _rigidbody.MoveRotation(transform.rotation * Quaternion.Euler(0f, Look.x * sensitivity, 0f));
    }

    private void FixedUpdate()
    {
        var velocity = _rigidbody.velocity;
        var horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

        var playerTransform = transform;
        var playerDirection = Direction.x * playerTransform.right + playerTransform.forward * Direction.y;
        playerDirection.Normalize();

        var finalForce = Vector3.zero;

        if (playerDirection != Vector3.zero)
        {
            var movementForce = playerDirection * acceleration;

            if (_grounded)
            {
                if (horizontalVelocity.magnitude <= maxSpeed)
                {
                    finalForce += movementForce;
                }
                else
                {
                    finalForce += (playerDirection - horizontalVelocity.normalized) * acceleration;
                }
            }
            else
            {
                movementForce *= airControl;
                finalForce += movementForce;
            }
        }
        else
        {
            var breakingForce = -horizontalVelocity * deceleration;

            if (!_grounded)
            {
                breakingForce *= airBrake;
            }

            finalForce += breakingForce;
        }

        //use debug draw rays to visualize the forces

        Debug.DrawRay(transform.position, playerDirection * 3, Color.red);
        Debug.DrawRay(transform.position, horizontalVelocity * 0.1f, Color.blue);
        Debug.DrawRay(transform.position, -horizontalVelocity * 0.1f, Color.yellow);
        Debug.DrawRay(transform.position, finalForce, Color.magenta);

        _rigidbody.AddForce(finalForce * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    private void Jump()
    {
        var jumpForce = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        Jumping = false;
    }
}