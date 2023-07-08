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

        [Tooltip("The amount of time the player has to wait before being able to jump again in s")]
        [SerializeField]
        private float jumpCooldown = 0.4f; // s

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
        private float rayLength = 1.1f; // m

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
        private bool _canJump = true;
        private bool _rayDidHit;
        private RaycastHit _hitInfo;

        private Vector2 _movementInput;

        private Rigidbody _rigidbody;

        /// <summary>
        ///     Whether or not the player can move.
        /// </summary>
        public bool MovementEnabled { get; set; } = true;

        /// <summary>
        ///     The movement input of the player.
        /// </summary>
        public Vector2 MovementInput
        {
            get => _movementInput;
            set => _movementInput = value.normalized;
        }

        /// <summary>
        ///     Whether or not the player is grounded.
        /// </summary>
        public bool IsGrounded { get; private set; }

        /// <summary>
        ///     The layer mask used to detect the ground.
        /// </summary>
        public LayerMask GroundLayerMask => groundLayerMask;

        /// <summary>
        ///     The velocity of the player.
        /// </summary>
        public Vector3 Velocity => _rigidbody.velocity;

        /// <summary>
        ///     The maximum speed of the player.
        /// </summary>
        public float MaxSpeed => maxSpeed;

        /// <summary>
        ///     Whether or not the player is stopping.
        /// </summary>
        public bool Stopping => MovementInput == Vector2.zero;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            OnJump += JumpAsync;
        }

        private void FixedUpdate()
        {
            if (MovementEnabled) MovementLogic();
            GroundCheck();
            if (_floatingEnabled) Floating();
        }

        /// <summary>
        ///     Makes the player jump.
        /// </summary>
        public void Jump()
        {
            if (IsGrounded && _canJump) OnJump?.Invoke();
        }

        private void MovementLogic()
        {
            var horizontalVelocity = new Vector3(Velocity.x, 0f, Velocity.z);

            var playerTransform = transform;
            var playerDirection = MovementInput.x * playerTransform.right + MovementInput.y * playerTransform.forward;
            playerDirection.Normalize();

            Vector3 finalForce;

            if (playerDirection != Vector3.zero)
            {
                var horizontalRemappedVelocity = horizontalVelocity.normalized *
                                                 Mathf.Clamp01(horizontalVelocity.magnitude / maxSpeed);

                finalForce = playerDirection - horizontalRemappedVelocity;

                finalForce *= acceleration;
                finalForce *= IsGrounded ? 1 : airControl;
            }
            else
            {
                finalForce = -horizontalVelocity * deceleration;

                finalForce *= IsGrounded ? 1 : airBrake;
            }

            _rigidbody.AddForce(finalForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

        private void GroundCheck()
        {
            var ray = new Ray(transform.position, Vector3.down);
            _rayDidHit = Physics.Raycast(ray, out _hitInfo, rayLength, groundLayerMask);

            var wasGrounded = IsGrounded;
            IsGrounded = _rayDidHit && _hitInfo.distance <= rideHeight;

            if (IsGrounded && !wasGrounded) OnLand?.Invoke(_rigidbody.velocity.y);
        }

        private void Floating()
        {
            if (!_rayDidHit) return;

            var hitBody = _hitInfo.rigidbody;

            var otherVel = hitBody ? hitBody.velocity : Vector3.zero;

            var rayDirVel = Vector3.Dot(Vector3.down, _rigidbody.velocity);
            var otherDirVel = Vector3.Dot(Vector3.down, otherVel);

            var relVel = rayDirVel - otherDirVel;

            var x = _hitInfo.distance - rideHeight;

            var springForce = x * rideSpringStrength - relVel * rideSpringDamper;

            _rigidbody.AddForce(Vector3.down * springForce, ForceMode.Acceleration);

            if (hitBody)
                hitBody.AddForceAtPosition(Vector3.up * springForce, _hitInfo.point, ForceMode.Acceleration);
        }

        /// <summary>
        ///     Invoked when the player jumps.
        /// </summary>
        public event UnityAction OnJump;

        /// <summary>
        ///     Invoked when the player lands.
        /// </summary>
        /// <remarks>
        ///    The float represents the y velocity of the player when they landed.
        ///    This can be used to play a landing animation.
        /// </remarks>
        public event UnityAction<float> OnLand;

        private async void JumpAsync()
        {
            _floatingEnabled = false;
            _canJump = false;

            _rigidbody.velocity = new Vector3(Velocity.x, 0f, Velocity.z);

            var jumpForce = Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            _rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);

            await Task.Delay(TimeSpan.FromSeconds(jumpCooldown));

            _floatingEnabled = true;
            _canJump = true;
        }
    }
}