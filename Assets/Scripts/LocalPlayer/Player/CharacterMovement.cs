using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace LocalPlayer.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMovement : NetworkBehaviour
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

        private NetworkRole _networkRole;



        private const int BUFFER_SIZE = 1024;
        private InputPayload[] _inputBuffer = new InputPayload[BUFFER_SIZE];
        private StatePayload[] _stateBuffer = new StatePayload[BUFFER_SIZE];

        private NetworkVariable<StatePayload> _latestState = new();
        private StatePayload _previousState = new();








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
        public Vector2 LookInput { get; set; }
        public bool Stopping => MovementInput == Vector2.zero;


        #region Logic

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            OnJump += CharacterMovement_OnJump;
        }

        private void FixedUpdate()
        {
            if (IsClient || IsServer) return;

            PerformMovement(MovementInput);
        }

        private void Update()
        {
            PerformRotation(LookInput);
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

        private void PerformRotation(Vector2 lookInput, float deltaTime = 1.0f)
        {
            RotateY(lookInput.x * deltaTime);
        }

        private void PerformMovement(Vector2 movementInput, float deltaTime = 1.0f)
        {
            if (MovementEnabled) Move(movementInput, deltaTime);
            ApplyGravity(gravityScale, deltaTime);
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

        public void RotateY(float angle)
        {
            Transform.Rotate(Vector3.up, angle);
        }

        private void ApplyGravity(float gravityScale, float deltaTime = 1.0f)
        {
            Rigidbody.AddForce((gravityScale - 1) * deltaTime * Physics.gravity, ForceMode.Acceleration);
        }

        public void Jump()
        {
            OnJump?.Invoke();
        }

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

        #endregion


        #region Networking

        public override void OnNetworkSpawn()
        {
            NetworkManager.NetworkTickSystem.Tick += NetworkTickSystem_Tick;
            _latestState.OnValueChanged += LatestState_OnValueChanged;
        }

        private void LatestState_OnValueChanged(StatePayload previousValue, StatePayload newValue)
        {
            _previousState = previousValue;

            if (Vector3.Distance(previousValue.Position, newValue.Position) > 0.001f)
            {
                float physicsFrequency = 1 / Time.fixedDeltaTime;
                float elapsedTicks = physicsFrequency / NetworkManager.NetworkTickSystem.TickRate;

                Transform.position = newValue.Position;
                Rigidbody.velocity = newValue.Velocity;

                for (var i = _latestState.Value.Tick; i < NetworkManager.LocalTime.Tick; i++)
                {
                    var input = _inputBuffer[i % BUFFER_SIZE];

                    PerformMovement(input.MoveInput, elapsedTicks);

                    StatePayload statePayload = new()
                    {
                        Tick = NetworkManager.LocalTime.Tick,
                        Position = Transform.position,
                        Rotation = Transform.rotation,
                        Velocity = Rigidbody.velocity,
                    };

                    _stateBuffer[NetworkManager.LocalTime.Tick % BUFFER_SIZE] = statePayload;
                }
            }
        }

        private void NetworkTickSystem_Tick()
        {
            float physicsFrequency = 1 / Time.fixedDeltaTime;
            float frameRate = 1 / Time.deltaTime;
            float elapsedTicks = physicsFrequency / NetworkManager.NetworkTickSystem.TickRate;

            if (IsOwner)
            {
                var bufferIndex = NetworkManager.LocalTime.Tick % BUFFER_SIZE;

                InputPayload inputPayload = new()
                {
                    Tick = NetworkManager.LocalTime.Tick,
                    MoveInput = MovementInput,
                    LookInput = LookInput,
                    JumpInput = false
                };

                StatePayload statePayload = new()
                {
                    Tick = NetworkManager.LocalTime.Tick,
                    Position = Transform.position,
                    Rotation = Transform.rotation,
                    Velocity = Rigidbody.velocity,
                };

                _inputBuffer[bufferIndex] = inputPayload;
                _stateBuffer[bufferIndex] = statePayload;

                PerformMovement(MovementInput, elapsedTicks);
                PerformMovementServerRpc(inputPayload);
            }
        }

        [ServerRpc]
        private void PerformMovementServerRpc(InputPayload inputPayload)
        {
            if (IsOwner) return;

            float physicsFrequency = 1 / Time.fixedDeltaTime;
            float elapsedSeconds = physicsFrequency / NetworkManager.NetworkTickSystem.TickRate;

            if (NetworkManager.LocalTime.Tick != _previousState.Tick)
            {
                // we've lost a tick, so we need to perform a correction
            }

            PerformMovement(inputPayload.MoveInput, elapsedSeconds);

            StatePayload statePayload = new()
            {
                Tick = NetworkManager.LocalTime.Tick,
                Position = Transform.position,
                Rotation = Transform.rotation,
                Velocity = Rigidbody.velocity,
            };

            _previousState = _latestState.Value;
            _latestState.Value = statePayload;
        }

        public override void OnNetworkDespawn()
        {
            NetworkManager.NetworkTickSystem.Tick -= NetworkTickSystem_Tick;
            _latestState.OnValueChanged -= LatestState_OnValueChanged;
        }

        #endregion

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