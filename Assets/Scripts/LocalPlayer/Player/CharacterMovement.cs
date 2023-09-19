using Cinemachine;
using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace LocalPlayer.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMovement : NetworkBehaviour
    {
        [field: Header("Movement")]
        [field: SerializeField]
        public float Acceleration { get; set; } = 64f;

        [field: SerializeField]
        public float Deceleartion { get; set; } = 128f;

        [field: SerializeField]
        public float MaxSpeed { get; set; } = 13f;

        [field: Space]
        [field: Header("Jump")]
        [field: SerializeField]
        public float JumpHeight { get; set; } = 3f;

        [field: SerializeField]
        private float GravityScale { get; set; } = 1.5f;

        [field: SerializeField]
        [field: Range(0, 1f)]
        public float AirControl { get; set; } = 0.1f;

        [field: SerializeField]
        [field: Range(0, 1f)]
        public float AirBrake { get; set; } = 0f;

        [field: Space]
        [field: Header("Grounding")]
        [field: SerializeField]
        public LayerMask GroundLayerMask { get; set; }

        [field: Space]
        [field: Header("Camera")]
        [field: SerializeField]
        public CinemachineVirtualCamera CinemachineVirtualCamera { get; set; }

        private const int BUFFER_SIZE = 1024;
        private readonly InputPayload[] _inputBuffer = new InputPayload[BUFFER_SIZE];
        private readonly StatePayload[] _stateBuffer = new StatePayload[BUFFER_SIZE];

        private readonly NetworkVariable<StatePayload> _latestState = new();
        private StatePayload _previousState = new();

        public Rigidbody Rigidbody { get; private set; }
        public bool IsGrounded { get; private set; }
        public Vector3 Velocity => Rigidbody ? Rigidbody.velocity : Vector3.zero;
        public bool MovementEnabled { get; set; } = true;
        public Vector2 MovementInput { get; set; }
        public bool Stopping => MovementInput == Vector2.zero;

        public delegate void OnLandedCallback(float fallSpeed);

        public event UnityAction OnJump;
        public event OnLandedCallback OnLanded;

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
            PerformRotation();
        }

        private void OnCollisionStay(Collision collision)
        {
            IsGrounded = collision.contacts.Any(contactPoint => Vector3.Dot(contactPoint.normal, Vector3.up) > 0.5f);
        }

        private void OnCollisionExit(Collision collision)
        {
            IsGrounded = false;
        }

        private void PerformRotation()
        {
            Rigidbody.rotation = Quaternion.Euler(0, CinemachineVirtualCamera.transform.eulerAngles.y, 0);
        }

        private void PerformMovement(Vector2 movementInput, float deltaTime = 1.0f)
        {
            if (MovementEnabled) Move(movementInput, deltaTime);
            ApplyGravity(GravityScale, deltaTime);
        }

        private void Move(Vector2 movementInput, float deltaTime = 1.0f)
        {
            Vector3 horizontalVelocity = new(Velocity.x, 0, Velocity.z);

            Vector3 moveDirection = movementInput.x * transform.right + movementInput.y * transform.forward;
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
            transform.Rotate(Vector3.up, angle);
        }

        private void ApplyGravity(float gravityScale, float deltaTime = 1.0f)
        {
            Rigidbody.AddForce((gravityScale - 1) * deltaTime * Physics.gravity, ForceMode.Acceleration);
        }

        public void Jump()
        {
            OnJump?.Invoke();
        }

        private void CharacterMovement_OnJump()
        {
            if (!IsGrounded) return;

            Rigidbody.velocity = new(Velocity.x, 0, Velocity.z);

            Vector3 jumpForce = new()
            {
                y = Mathf.Sqrt(-2 * Physics.gravity.y * GravityScale * JumpHeight)
            };

            Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
        }

        public override void OnNetworkSpawn()
        {
            NetworkManager.NetworkTickSystem.Tick += NetworkTickSystem_Tick;
            _latestState.OnValueChanged += LatestState_OnValueChanged;
        }

        public override void OnNetworkDespawn()
        {
            NetworkManager.NetworkTickSystem.Tick -= NetworkTickSystem_Tick;
            _latestState.OnValueChanged -= LatestState_OnValueChanged;
        }

        private void LatestState_OnValueChanged(StatePayload previousValue, StatePayload newValue)
        {
            _previousState = previousValue;

            if (Vector3.Distance(previousValue.Position, newValue.Position) > 0.001f)
            {
                float physicsFrequency = 1 / Time.fixedDeltaTime;
                float elapsedTicks = physicsFrequency / NetworkManager.NetworkTickSystem.TickRate;

                transform.position = newValue.Position;
                Rigidbody.velocity = newValue.Velocity;

                for (var i = _latestState.Value.Tick; i < NetworkManager.LocalTime.Tick; i++)
                {
                    var input = _inputBuffer[i % BUFFER_SIZE];

                    PerformMovement(input.MoveInput, elapsedTicks);
                    PerformRotation();

                    StatePayload statePayload = new()
                    {
                        Tick = NetworkManager.LocalTime.Tick,
                        Position = transform.position,
                        Rotation = transform.rotation,
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
                    JumpInput = false
                };

                StatePayload statePayload = new()
                {
                    Tick = NetworkManager.LocalTime.Tick,
                    Position = transform.position,
                    Rotation = transform.rotation,
                    Velocity = Rigidbody.velocity,
                };

                _inputBuffer[bufferIndex] = inputPayload;
                _stateBuffer[bufferIndex] = statePayload;

                PerformRotation();
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
                transform.SetPositionAndRotation(_previousState.Position, _previousState.Rotation);
                Rigidbody.velocity = _previousState.Velocity;
            }

            PerformRotation();
            PerformMovement(inputPayload.MoveInput, elapsedSeconds);

            StatePayload statePayload = new()
            {
                Tick = NetworkManager.LocalTime.Tick,
                Position = transform.position,
                Rotation = transform.rotation,
                Velocity = Rigidbody.velocity,
            };

            _previousState = _latestState.Value;
            _latestState.Value = statePayload;
        }
    }
}