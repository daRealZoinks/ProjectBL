using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace LocalPlayer
{
    [RequireComponent(typeof(PlayerCharacterController))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerWallRunController : MonoBehaviour
    {
        [Tooltip("The distance at which the player character checks for walls to wall run on.")]
        [Range(0.1f, 2f)]
        [SerializeField]
        private float wallCheckDistance = 0.75f;

        [Tooltip("The amount of time the player character has to wait before being able to wall run again.")]
        [SerializeField]
        [Range(0.1f, 1f)]
        private float wallRunCooldown = 0.4f;

        [Space]
        [Header("Wall Run Settings")]
        [Tooltip("The height of the jump when the player jumps off a wall.")]
        [SerializeField]
        private float wallJumpHeight = 3f;

        [Tooltip("The amount of forward impulse applied to the player character when starting a wall run.")]
        [SerializeField]
        private float wallRunInitialImpulse = 4f;

        [Space]
        [Header("Wall Jump Settings")]
        [Tooltip("The amount of force applied to the player character when jumping off a wall to the side.")]
        [SerializeField]
        private float wallJumpSideForce = 7f;

        [Tooltip("The amount of forward force applied to the player character when jumping off a wall.")]
        [SerializeField]
        private float wallJumpForwardForce = 4f;

        private PlayerCharacterController _playerCharacterController;

        private RaycastHit _leftHitInfo;
        private RaycastHit _rightHitInfo;

        private bool _canWallJump = true;

        private Rigidbody _rigidbody;
        private bool _wallRunningEnabled = true;

        /// <summary>
        ///     Whether the player character is currently running on a wall to the right.
        /// </summary>
        public bool IsWallRight { get; private set; }

        /// <summary>
        ///     Whether the player character is currently running on a wall to the left.
        /// </summary>
        public bool IsWallLeft { get; private set; }

        /// <summary>
        ///     Whether the player character is currently running on a wall.
        /// </summary>
        public bool IsWallRunning => IsWallRight || IsWallLeft;

        private void Awake()
        {
            _playerCharacterController = GetComponent<PlayerCharacterController>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            OnWallJump += WallJumpAsync;
        }

        public void WallJump()
        {
            if (IsWallRunning && _canWallJump) OnWallJump?.Invoke();
        }

        private void Update()
        {
            // Disable movement if the player character is wall running.
            // This is to prevent the player character from moving away from the wall.
            _playerCharacterController.MovementEnabled = !IsWallRunning;
        }

        private void FixedUpdate()
        {
            if (!_wallRunningEnabled) return;

            if (_playerCharacterController.IsGrounded)
            {
                IsWallRight = false;
                IsWallLeft = false;
                return;
            }

            var playerTransform = transform;
            var playerTransformPosition = playerTransform.position;
            var playerTransformRight = playerTransform.right;

            Ray rightRay = new(playerTransformPosition, playerTransformRight);
            Ray leftRay = new(playerTransformPosition, -playerTransformRight);

            var wasWallRight = IsWallRight;
            var wasWallLeft = IsWallLeft;

            if (!IsWallRight)
                IsWallRight = Physics.Raycast(rightRay, out _rightHitInfo, wallCheckDistance,
                    _playerCharacterController.GroundLayerMask);

            if (!IsWallLeft)
                IsWallLeft = Physics.Raycast(leftRay, out _leftHitInfo, wallCheckDistance,
                    _playerCharacterController.GroundLayerMask);


            Vector3 boostForce = Vector3.zero;

            var velocity = _playerCharacterController.Velocity;

            // take into account the direction the player is sliding along the wall
            if (IsWallRight && !wasWallRight)
            {
                boostForce = -Vector3.Cross(_rightHitInfo.normal, transform.up) * wallRunInitialImpulse;
            }

            if (IsWallLeft && !wasWallLeft)
            {
                boostForce = Vector3.Cross(_leftHitInfo.normal, transform.up) * wallRunInitialImpulse;
            }

            _rigidbody.AddForce(boostForce, ForceMode.VelocityChange);


            if (IsWallRight) _rigidbody.AddForce(-_rightHitInfo.normal, ForceMode.Acceleration);
            if (IsWallLeft) _rigidbody.AddForce(-_leftHitInfo.normal, ForceMode.Acceleration);
        }

        /// <summary>
        ///     Event invoked when the player character jumps off a wall.
        /// </summary>
        public event UnityAction OnWallJump;

        private async void WallJumpAsync()
        {
            var sideForce = Vector3.zero;
            if (IsWallRight) sideForce = _rightHitInfo.normal * wallJumpSideForce;
            if (IsWallLeft) sideForce = _leftHitInfo.normal * wallJumpSideForce;

            var jumpForce = Vector3.up * Mathf.Sqrt(wallJumpHeight * -2 * Physics.gravity.y);

            var forwardForce = transform.forward * wallJumpForwardForce;

            var finalForce = jumpForce + sideForce + forwardForce;

            _wallRunningEnabled = false;
            _canWallJump = false;

            var rigidbodyVelocity = _rigidbody.velocity;
            rigidbodyVelocity = new Vector3(rigidbodyVelocity.x, 0f, rigidbodyVelocity.z);
            _rigidbody.velocity = rigidbodyVelocity;

            if (IsWallRight)
            {
                var velocity = Vector3.Project(_rigidbody.velocity, _rightHitInfo.normal);

                _rigidbody.velocity -= velocity;
            }

            if (IsWallLeft)
            {
                var velocity = Vector3.Project(_rigidbody.velocity, _leftHitInfo.normal);

                _rigidbody.velocity -= velocity;
            }

            IsWallRight = false;
            IsWallLeft = false;

            _rigidbody.AddForce(finalForce, ForceMode.VelocityChange);

            await Task.Delay(TimeSpan.FromSeconds(wallRunCooldown));

            _wallRunningEnabled = true;
            _canWallJump = true;
        }
    }
}