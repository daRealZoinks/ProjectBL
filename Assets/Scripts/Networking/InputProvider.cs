using LocalPlayer;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Networking
{
    [RequireComponent(typeof(PlayerCharacterController))]
    [RequireComponent(typeof(PlayerWallRunController))]
    public class InputProvider : NetworkBehaviour
    {
        [Tooltip("The movement input threshold")]
        [SerializeField]
        private float threshold = 0.001f;

        [Tooltip("The player camera controller")]
        [SerializeField]
        private PlayerCameraController playerCameraController;

        [Tooltip("The melee attack")]
        [SerializeField]
        private MeleeKick meleeKick;

        [Tooltip("The weapon manager")]
        [SerializeField]
        private WeaponManager weaponManager;

        private PlayerCharacterController _playerCharacterController;
        private PlayerWallRunController _playerWallRunController;
        private Rigidbody _rigidbody;

        // Client side prediction
        private float _timer;
        private int _currentTick;
        private float _minTimeBetweenTicks;

        private const int BUFFER_SIZE = 1024;

        // Client specific
        private Vector2 _movementInput;
        private Vector2 _lookInput;
        private bool _jumpInput;

        private StatePayload[] _clientStateBuffer;
        private InputPayload[] _inputBuffer;
        private StatePayload _latestServerState;
        private StatePayload _lastProcessedState;

        // Server specific
        private StatePayload[] _serverStateBuffer;
        private Queue<InputPayload> _inputQueue;

        private void Awake()
        {
            _playerCharacterController = GetComponent<PlayerCharacterController>();
            _playerWallRunController = GetComponent<PlayerWallRunController>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _minTimeBetweenTicks = 1f / NetworkManager.Singleton.NetworkTickSystem.TickRate;

            _clientStateBuffer = new StatePayload[BUFFER_SIZE];
            _serverStateBuffer = new StatePayload[BUFFER_SIZE];

            _inputBuffer = new InputPayload[BUFFER_SIZE];

            _inputQueue = new();
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            while (_timer >= _minTimeBetweenTicks)
            {
                _timer -= _minTimeBetweenTicks;
                HandleTick();
                _currentTick++;
            }
        }

        private void HandleTick()
        {
            bool isLatestServerStateDefault = _latestServerState.Equals(default(StatePayload));
            bool isLastProcessedStateDefault = _lastProcessedState.Equals(default(StatePayload));
            bool updateNeeded = !_latestServerState.Equals(_lastProcessedState);

            if (!isLatestServerStateDefault && (isLastProcessedStateDefault || updateNeeded))
            {
                HandleServerReconciliation();
            }

            // TODO: figure out how to make the host be able to work

            if (IsOwner) HandleClientInput();

            if (IsServer) HandleServerInput();
        }

        private void HandleServerReconciliation()
        {
            _lastProcessedState = _latestServerState;

            int serverStateBufferIndex = _latestServerState.Tick % BUFFER_SIZE;
            float positionError = Vector3.Distance(_latestServerState.Position, _clientStateBuffer[serverStateBufferIndex].Position);

            if (positionError <= threshold) return;

            transform.position = _latestServerState.Position;
            _rigidbody.velocity = _latestServerState.Velocity;

            _clientStateBuffer[serverStateBufferIndex] = _latestServerState;

            for (int tickToProcess = _latestServerState.Tick + 1; tickToProcess <= _currentTick; tickToProcess++)
            {
                StatePayload statePayload = ProcessMovement(_inputBuffer[tickToProcess]);

                int bufferIndex = tickToProcess % BUFFER_SIZE;
                _clientStateBuffer[bufferIndex] = statePayload;
            }
        }

        private void HandleClientInput()
        {
            var bufferIndex = _currentTick % BUFFER_SIZE;

            InputPayload inputPayload = new()
            {
                Tick = _currentTick,
                MoveInput = _movementInput,
                LookInput = _lookInput
            };

            if (_jumpInput)
            {
                inputPayload.JumpInput = true;
                _jumpInput = false;
            }

            _inputBuffer[bufferIndex] = inputPayload;

            _clientStateBuffer[bufferIndex] = ProcessMovement(inputPayload);

            SendInputServerRpc(inputPayload);
        }

        [ServerRpc]
        private void SendInputServerRpc(InputPayload inputPayload)
        {
            _inputQueue.Enqueue(inputPayload);
        }

        private void HandleServerInput()
        {
            var bufferIndex = -1;
            while (_inputQueue.Count > 0)
            {
                InputPayload inputPayload = _inputQueue.Dequeue();

                bufferIndex = inputPayload.Tick % BUFFER_SIZE;
                StatePayload statePayload = ProcessMovement(inputPayload);
                _serverStateBuffer[bufferIndex] = statePayload;
            }

            if (bufferIndex != -1)
            {
                SendStateClientRpc(_serverStateBuffer[bufferIndex]);
            }
        }

        [ClientRpc]
        private void SendStateClientRpc(StatePayload statePayload)
        {
            _latestServerState = statePayload;
        }

        private StatePayload ProcessMovement(InputPayload inputPayload)
        {
            _playerCharacterController.MovementInput = inputPayload.MoveInput;
            playerCameraController.LookInput = inputPayload.LookInput;

            if (inputPayload.JumpInput)
            {
                _playerCharacterController.Jump();
                _playerWallRunController.WallJump();
            }

            return new()
            {
                Tick = inputPayload.Tick,
                Position = transform.position,
                Rotation = transform.rotation,
                Velocity = _rigidbody.velocity
            };
        }

        /// <summary>
        ///     Called when the player moves
        /// </summary>
        /// <param name="context"> The move input context </param>
        public void OnMove(InputAction.CallbackContext context)
        {
            var value = context.phase switch
            {
                InputActionPhase.Started or InputActionPhase.Performed => context.ReadValue<Vector2>(),
                _ => Vector2.zero
            };

            _movementInput = value;
        }

        /// <summary>
        ///     Called when the player looks
        /// </summary>
        /// <param name="context"> The look input context </param>
        public void OnLook(InputAction.CallbackContext context)
        {
            var value = context.phase switch
            {
                InputActionPhase.Started or InputActionPhase.Performed => context.ReadValue<Vector2>(),
                _ => Vector2.zero
            };

            _lookInput = value;
        }

        /// <summary>
        ///     Called when the player jumps
        /// </summary>
        /// <param name="context"> The jump input context </param>
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            _jumpInput = true;
        }

        /// <summary>
        ///     Called when the player fires
        /// </summary>
        /// <param name="context"> The fire input context </param>
        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            if (IsOwner) ShootServerRpc();

            weaponManager.Shoot();
        }

        [ServerRpc]
        private void ShootServerRpc()
        {
            ShootClientRpc();
            weaponManager.Shoot();
        }

        [ClientRpc]
        private void ShootClientRpc()
        {
            if (!IsOwner) weaponManager.Shoot();
        }

        /// <summary>
        ///     Called when the player sprints
        /// </summary>
        /// <param name="context"> The melee input context </param>
        public void OnMelee(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            if (IsOwner) MeleeServerRpc();

            meleeKick.Attack();
        }

        [ServerRpc]
        private void MeleeServerRpc()
        {
            MeleeClientRpc();
            meleeKick.Attack();
        }

        [ClientRpc]
        private void MeleeClientRpc()
        {
            if (!IsOwner) meleeKick.Attack();
        }

        private struct StatePayload : INetworkSerializable
        {
            private int _tick;
            private Vector3 _position;
            private Quaternion _rotation;
            private Vector3 _velocity;

            internal int Tick
            {
                readonly get => _tick;
                set => _tick = value;
            }
            internal Vector3 Position
            {
                readonly get => _position;
                set => _position = value;
            }
            internal Quaternion Rotation
            {
                readonly get => _rotation;
                set => _rotation = value;
            }
            internal Vector3 Velocity
            {
                readonly get => _velocity;
                set => _velocity = value;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref _tick);
                serializer.SerializeValue(ref _position);
                serializer.SerializeValue(ref _rotation);
                serializer.SerializeValue(ref _velocity);
            }
        }

        private struct InputPayload : INetworkSerializable
        {
            private int _tick;
            private Vector2 _moveInput;
            private Vector2 _lookInput;
            private bool _jumpInput;

            internal int Tick
            {
                readonly get => _tick;
                set => _tick = value;
            }
            internal Vector2 MoveInput
            {
                readonly get => _moveInput;
                set => _moveInput = value;
            }
            internal Vector2 LookInput
            {
                readonly get => _lookInput;
                set => _lookInput = value;
            }
            internal bool JumpInput
            {
                readonly get => _jumpInput;
                set => _jumpInput = value;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref _tick);
                serializer.SerializeValue(ref _moveInput);
                serializer.SerializeValue(ref _lookInput);
                serializer.SerializeValue(ref _jumpInput);
            }
        }
    }
}