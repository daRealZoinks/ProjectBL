using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace LocalPlayer.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class CharacterMovement : NetworkBehaviour
    {
        [Header("Movement")]
        [SerializeField]
        private float acceleration = 64f; // m/s

        [SerializeField]
        private float deceleartion = 128f; // m/s

        [SerializeField]
        private float maxSpeed = 13f; // m/s

        [Space]
        [Header("Jump")]
        [SerializeField]
        private float jumpHeight = 3f; // m

        [SerializeField]
        private float gravityScale = 1.5f;

        [SerializeField]
        [Range(0, 1f)]
        private float airControl = 0.1f; // %

        [SerializeField]
        [Range(0, 1f)]
        private float airBrake = 0f; // %

        [Space]
        [Header("Grounding")]
        [SerializeField]
        private LayerMask groundLayerMask;

        public float Acceleration => acceleration;
        public float Deceleartion => deceleartion;
        public float MaxSpeed => maxSpeed;
        public float JumpHeight => jumpHeight;
        public float AirControl => airControl;
        public float AirBrake => airBrake;
        public LayerMask GroundLayerMask => groundLayerMask;

        public Transform Transform => transform;

        public Rigidbody Rigidbody { get; private set; }

        public bool IsGrounded { get; private set; }

        public Vector3 Velocity => Rigidbody ? Rigidbody.velocity : Vector3.zero;

        public bool MovementEnabled { get; set; } = true;

        public Vector2 MovementInput { get; set; }

        public bool Stopping => MovementInput == Vector2.zero;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            OnJump += CharacterMovement_OnJump;
        }

        private void OnCollisionStay(Collision collision)
        {
            IsGrounded = false;

            for (var i = 0; i < collision.contactCount; i++)
            {
                var contactPoint = collision.GetContact(i);

                if (Vector3.Dot(contactPoint.normal, Vector3.up) > 0.5f)
                {
                    IsGrounded = true;
                    break;
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            IsGrounded = false;
        }

        private void FixedUpdate()
        {
            if (MovementEnabled) Move(MovementInput);

            Rigidbody.AddForce(Physics.gravity * (gravityScale - 1), ForceMode.Acceleration);
        }

        private void Move(Vector2 movementInput, float deltaTime = 1.0f)
        {
            Vector3 horizontalVelocity = new(Velocity.x, 0, Velocity.z);

            Vector3 moveDirection = movementInput.x * Transform.right + movementInput.y * Transform.forward;
            moveDirection.Normalize();

            Vector3 horizontalClampedVelocity = horizontalVelocity.normalized *
                Mathf.Clamp01(horizontalVelocity.magnitude / MaxSpeed);

            Vector3 finalForce;

            if (moveDirection != Vector3.zero)
            {
                var accelerationForce = moveDirection - horizontalClampedVelocity;
                accelerationForce *= Acceleration * (IsGrounded ? 1 : AirControl);

                finalForce = accelerationForce;
            }
            else
            {
                var deceleartionForce = -horizontalClampedVelocity;
                deceleartionForce *= Deceleartion * (IsGrounded ? 1 : AirBrake);

                finalForce = deceleartionForce;
            }

            Rigidbody.AddForce(finalForce * deltaTime, ForceMode.Acceleration);
        }

        public void Jump()
        {
            OnJump?.Invoke();
        }

        //an on land delegate 
        public delegate void OnLandedCallback(float fallSpeed);

        public event UnityAction OnJump;

        public event OnLandedCallback OnLanded;

        private void CharacterMovement_OnJump()
        {
            if (!IsGrounded) return;

            Rigidbody.velocity = new(Velocity.x, 0, Velocity.z);

            Vector3 jumpForce = new()
            {
                y = Mathf.Sqrt(-2 * Physics.gravity.y * gravityScale * jumpHeight)
            };

            Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
        }

        private void OnDrawGizmos()
        {
            // Draw the velocity
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Transform.position, Transform.position + Velocity);

            // Draw the movement input
            Gizmos.color = Color.blue;
            Vector3 movementInputLine = MovementInput.x * Transform.right + MovementInput.y * Transform.forward;
            Gizmos.DrawLine(Transform.position, Transform.position + movementInputLine);

            // Draw the ground check
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(Transform.position, 0.1f);
        }
    }
}