using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace LocalPlayer
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerCharacterController : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("The acceleration of the player in m/s^2")]
        [SerializeField]
        private float acceleration = 4096f; // m/s^2

        [Tooltip("The deceleration of the player in m/s^2")]
        [SerializeField]
        private float deceleration = 700f; // m/s^2

        [Tooltip("The maximum speed of the player in m/s")]
        [SerializeField]
        private float maxSpeed = 13f; // m/s

        [Space]
        [Header("Jump")]
        [Tooltip("The height of the player's jump in m")]
        [SerializeField]
        private float jumpHeight = 3f; // m

        [Tooltip("The amount of control the player has in the air")]
        [SerializeField]
        [Range(0, 1f)]
        private float airControl = 0.1f; // 0-1 (0 = no control, 1 = full control)

        [Tooltip("The amount of air braking the player has")]
        [SerializeField]
        [Range(0, 1f)]
        private float airBrake; // 0-1 (0 = no brake, 1 = full brake)

        [Space]
        [Header("Grounding")]
        [Tooltip("The height of the player's capsule in m")]
        [SerializeField]
        private float rideHeight = 1f; // m

        [Tooltip("The length of the raycast used to detect the ground in m")]
        [SerializeField]
        private float rayLength = 1.5f; // m

        [Tooltip("The strength of the spring used to keep the player grounded in N/m")]
        [SerializeField]
        private float rideSpringStrength = 100f; // N/m

        [Tooltip("The damper of the spring used to keep the player grounded in N/(m/s)")]
        [SerializeField]
        private float rideSpringDamper = 10f; // N/(m/s)

        [Tooltip("The layer mask used to detect the ground")]
        [SerializeField]
        private LayerMask groundLayerMask;

        private bool _floatingEnabled = true;

        private Rigidbody _rigidbody;
        private Vector2 _direction;

        public Vector2 Direction
        {
            get => _direction;
            set => _direction = value.normalized;
        }
        public bool Jumping { get; set; }
        public bool Grounded { get; private set; }

        // Properties
        public Vector3 Movement => _rigidbody.velocity;
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

        private void Update()
        {
            if (Jumping && Grounded) OnJump?.Invoke();
        }

        private void FixedUpdate()
        {
            MovementLogic();
            Floating();
        }

        private void MovementLogic()
        {
            var velocity = _rigidbody.velocity;
            var horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

            var playerTransform = transform;
            var playerDirection = Direction.x * playerTransform.right + Direction.y * playerTransform.forward;
            playerDirection.Normalize();

            var finalForce = Vector3.zero;

            if (playerDirection != Vector3.zero)
            {
                var movementForce = playerDirection * acceleration * (Grounded ? 1 : airControl);

                if (horizontalVelocity.magnitude <= maxSpeed)
                    finalForce += movementForce;
                else
                    finalForce += (playerDirection - horizontalVelocity.normalized) * acceleration;
            }
            else
            {
                var breakingForce = -horizontalVelocity * deceleration;

                if (!Grounded) breakingForce *= airBrake;

                finalForce += breakingForce;
            }

            _rigidbody.AddForce(finalForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

        private void Floating()
        {
            var ray = new Ray(transform.position, Vector3.down);
            var rayDidHit = Physics.Raycast(ray, out var hitInfo, rayLength, groundLayerMask);

            var wasGrounded = Grounded;
            Grounded = rayDidHit && hitInfo.distance <= rideHeight;

            if (!_floatingEnabled) return;

            if (Grounded && !wasGrounded) OnLand?.Invoke(_rigidbody.velocity.y);

            if (rayDidHit)
            {
                var otherVel = Vector3.zero;
                var hitBody = hitInfo.rigidbody;
                if (hitBody != null) otherVel = hitBody.velocity;

                var rayDirVel = Vector3.Dot(Vector3.down, _rigidbody.velocity);
                var otherDirVel = Vector3.Dot(Vector3.down, otherVel);

                var relVel = rayDirVel - otherDirVel;

                var x = hitInfo.distance - rideHeight;

                var springForce = x * rideSpringStrength - relVel * rideSpringDamper;

                _rigidbody.AddForce(Vector3.down * springForce, ForceMode.Acceleration);

                if (hitBody != null)
                    hitBody.AddForceAtPosition(Vector3.up * springForce, hitInfo.point, ForceMode.Acceleration);
            }
        }

        // Events
        public event UnityAction OnJump;
        public event UnityAction<float> OnLand; // the float represents the y velocity of the player when they landed

        private async void Jump()
        {
            var jumpForce = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);

            _floatingEnabled = false;

            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            Jumping = false;
            await Task.Delay(TimeSpan.FromSeconds(0.4f));
            _floatingEnabled = true;
        }
    }
}