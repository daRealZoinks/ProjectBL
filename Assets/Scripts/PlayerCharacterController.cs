using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCharacterController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("The acceleration of the player in m/s^2")]
    [SerializeField] private float acceleration = 5120; // m/s^2
    [Tooltip("The deceleration of the player in m/s^2")]
    [SerializeField] private float deceleration = 800f; // m/s^2
    [Tooltip("The maximum speed of the player in m/s")]
    [SerializeField] private float maxSpeed = 15; // m/s

    [Space]
    [Header("Jump")]
    [Tooltip("The height of the player's jump in m")]
    [SerializeField] private float jumpHeight = 3f; // m
    [Tooltip("The amount of control the player has in the air")]
    [SerializeField] private float airControl = 0.05f; // 0-1 (0 = no control, 1 = full control)
    [Tooltip("The amount of air braking the player has")]
    [SerializeField] private float airBrake; // 0-1 (0 = no brake, 1 = full brake)
    [Tooltip("The layer mask for the ground")]
    [SerializeField] private LayerMask groundLayerMask;

    private Rigidbody _rigidbody;

    public Vector2 Direction { get; set; }
    public bool Jumping { get; set; }

    // Events
    public event UnityAction OnJump;
    public event UnityAction<float> OnLand; // the float is the magnitude of the impact

    // Properties
    public Vector3 Movement => _rigidbody.velocity;
    public bool Grounded { get; private set; }
    public float MaxSpeed => maxSpeed;
    public bool Stopping => Direction == Vector2.zero;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        OnJump += Jump;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (groundLayerMask == (groundLayerMask | (1 << other.gameObject.layer)))
        {
            Grounded = true;
            OnLand?.Invoke(other.impulse.magnitude);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (groundLayerMask == (groundLayerMask | (1 << other.gameObject.layer)))
        {
            Grounded = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        Grounded = false;
    }

    private void Update()
    {
        if (Jumping && Grounded)
        {
            OnJump?.Invoke();
        }
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

            if (Grounded)
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

            if (!Grounded)
            {
                breakingForce *= airBrake;
            }

            finalForce += breakingForce;
        }

        _rigidbody.AddForce(finalForce * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    private void Jump()
    {
        var jumpForce = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        Jumping = false;
    }
}