using System;
using System.Threading.Tasks;
using UnityEngine;

namespace LocalPlayer.Player
{
    [RequireComponent(typeof(CharacterMovement))]
    public class PlayerWallRunController : MonoBehaviour
    {
        [Tooltip("The distance at which the player character checks for walls to wall run on.")]
        [Range(0.1f, 2f)]
        [SerializeField]
        private float wallCheckDistance = 0.75f; // m

        [Tooltip("The amount of time the player character has to wait before being able to wall run again.")]
        [SerializeField]
        [Range(0.1f, 1f)]
        private float wallRunCooldown = 0.4f; // s

        [Space]
        [Header("Wall Run Settings")]
        [Tooltip("The height of the jump when the player jumps off a wall.")]
        [SerializeField]
        private float wallJumpHeight = 3f; // m

        [Tooltip("The amount of forward impulse applied to the player character when starting a wall run.")]
        [SerializeField]
        private float wallRunInitialImpulse = 4f; // m/s

        [Space]
        [Header("Wall Jump Settings")]
        [Tooltip("The amount of force applied to the player character when jumping off a wall to the side.")]
        [SerializeField]
        private float wallJumpSideForce = 7f; // m/s

        [Tooltip("The amount of forward force applied to the player character when jumping off a wall.")]
        [SerializeField]
        private float wallJumpForwardForce = 4f; // m/s

        private bool _canWallJump = true;

        private RaycastHit _leftHitInfo;

        private CharacterMovement _characterMovement;
        private RaycastHit _rightHitInfo;

        private bool _wallRunningEnabled = true;

        /// <summary>
        ///     Whether the player is currently running on a wall to the right.
        /// </summary>
        public bool IsWallRight { get; private set; }

        /// <summary>
        ///     Whether the player is currently running on a wall to the left.
        /// </summary>
        public bool IsWallLeft { get; private set; }

        /// <summary>
        ///     Whether the player is currently running on a wall.
        /// </summary>
        public bool IsWallRunning => IsWallRight || IsWallLeft;

        private void Awake()
        {
            _characterMovement = GetComponent<CharacterMovement>();
        }

        private void Start()
        {
            _characterMovement.OnJump += WallJumpAsync;
        }

        private void Update()
        {
            // Disable movement if the player character is wall running.
            // This is to prevent the player character from moving away from the wall.
            _characterMovement.MovementEnabled = !IsWallRunning;
        }

        private void FixedUpdate()
        {
            if (!_wallRunningEnabled) return;

            if (_characterMovement.IsGrounded)
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

            IsWallRight = Physics.Raycast(rightRay, out _rightHitInfo, wallCheckDistance,
                _characterMovement.GroundLayerMask);
            IsWallLeft = Physics.Raycast(leftRay, out _leftHitInfo, wallCheckDistance,
                _characterMovement.GroundLayerMask);

            var boostForce = Vector3.zero;

            if (IsWallRight && !wasWallRight)
                boostForce = -Vector3.Cross(_rightHitInfo.normal, transform.up) * wallRunInitialImpulse;

            if (IsWallLeft && !wasWallLeft)
                boostForce = Vector3.Cross(_leftHitInfo.normal, transform.up) * wallRunInitialImpulse;

            _characterMovement.Rigidbody.AddForce(boostForce, ForceMode.VelocityChange);

            if (IsWallRight) _characterMovement.Rigidbody.AddForce(-_rightHitInfo.normal, ForceMode.Acceleration);
            if (IsWallLeft) _characterMovement.Rigidbody.AddForce(-_leftHitInfo.normal, ForceMode.Acceleration);
        }

        private void OnDrawGizmos()
        {
            var playerTransform = transform;
            var playerTransformPosition = playerTransform.position;
            var playerTransformRight = playerTransform.right;

            Ray rightRay = new(playerTransformPosition, playerTransformRight);
            Ray leftRay = new(playerTransformPosition, -playerTransformRight);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(rightRay.origin, rightRay.direction * wallCheckDistance);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(leftRay.origin, leftRay.direction * wallCheckDistance);

            if (IsWallRight)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_rightHitInfo.point, 0.1f);
                Gizmos.DrawRay(_rightHitInfo.point, _rightHitInfo.normal);
            }

            if (IsWallLeft)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(_leftHitInfo.point, 0.1f);
                Gizmos.DrawRay(_leftHitInfo.point, _leftHitInfo.normal);
            }
        }

        private async void WallJumpAsync()
        {
            if (!(IsWallRunning && _canWallJump)) return;

            var sideForce = Vector3.zero;
            if (IsWallRight) sideForce = _rightHitInfo.normal * wallJumpSideForce;
            if (IsWallLeft) sideForce = _leftHitInfo.normal * wallJumpSideForce;

            var jumpForce = Vector3.up * Mathf.Sqrt(wallJumpHeight * -2 * Physics.gravity.y);

            var forwardForce = transform.forward * wallJumpForwardForce;

            var finalForce = jumpForce + sideForce + forwardForce;

            _wallRunningEnabled = false;
            _canWallJump = false;

            var rigidbodyVelocity = _characterMovement.Rigidbody.velocity;
            rigidbodyVelocity = new Vector3(rigidbodyVelocity.x, 0f, rigidbodyVelocity.z);
            _characterMovement.Rigidbody.velocity = rigidbodyVelocity;

            if (IsWallRight)
            {
                var velocity = Vector3.Project(_characterMovement.Rigidbody.velocity, _rightHitInfo.normal);

                _characterMovement.Rigidbody.velocity -= velocity;
            }

            if (IsWallLeft)
            {
                var velocity = Vector3.Project(_characterMovement.Rigidbody.velocity, _leftHitInfo.normal);

                _characterMovement.Rigidbody.velocity -= velocity;
            }

            IsWallRight = false;
            IsWallLeft = false;

            _characterMovement.Rigidbody.AddForce(finalForce, ForceMode.VelocityChange);

            await Task.Delay(TimeSpan.FromSeconds(wallRunCooldown));

            _wallRunningEnabled = true;
            _canWallJump = true;
        }
    }
}