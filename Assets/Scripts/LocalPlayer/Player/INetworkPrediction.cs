using Unity.Netcode;
using UnityEngine;

namespace LocalPlayer.Player
{
    public interface INetworkPrediction
    {
        // Server hooks

        /// <summary>
        ///     (Server) Send position to client if necessary, or just ack good moves.
        /// </summary>
        void SendClientAdjustment();


        /// <summary>
        ///     (Server) Trigger a position update on clients, if the server hasn't heard from them in a while.
        /// </summary>
        /// <param name="deltaTime"> The time between ticks </param>
        /// <returns> Whether movement is performed. </returns>
        bool ForcePositionUpdate(float deltaTime);


        // Client hooks

        /// <summary>
        ///     (Client) After receiving a network update of position, allow some custom smoothing, given the old
        ///     transform before the correction and new transform from the update.
        /// </summary>
        /// <param name="oldPosition"> The position of the client </param>
        /// <param name="oldRotation"> The rotation of the client </param>
        /// <param name="newPosition"> The position of the server given to the client </param>
        /// <param name="newRotation"> The rotation of the server given to the client </param>
        void SmoothCorrection(Vector3 oldPosition, Quaternion oldRotation, Vector3 newPosition, Quaternion newRotation);


        // Others

        /// <summary>
        ///     The data the client has predicted
        /// </summary>
        StatePayload PredictionData_Client { get; }


        /// <summary>
        ///     The data the server has predicted
        /// </summary>
        StatePayload PredictionData_Server { get; }


        /// <summary>
        ///     Accessor to check if there is already client data, without potentially allocating it on demand.
        /// </summary>
        /// <returns> If there is already client data. </returns>
        bool HasPredictionData_Client();


        /// <summary>
        ///     Accessor to check if there is already server data, without potentially allocating it on demand.
        /// </summary>
        /// <returns> If there is already server data. </returns>
        bool HasPredictionData_Server();


        /// <summary>
        ///     Resets client prediction data.
        /// </summary>
        void ResetPredictionData_Client();


        /// <summary>
        ///     Resets server prediction data.
        /// </summary>
        void ResetPredictionData_Server();
    }

    public struct InputPayload : INetworkSerializable
    {
        private int _tick;
        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private bool _jumpInput;

        public int Tick
        {
            readonly get => _tick;
            set => _tick = value;
        }
        public Vector2 MoveInput
        {
            readonly get => _moveInput;
            set => _moveInput = value;
        }
        public Vector2 LookInput
        {
            readonly get => _lookInput;
            set => _lookInput = value;
        }
        public bool JumpInput
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

    public struct StatePayload : INetworkSerializable
    {
        private int _tick;
        private Vector3 _position;
        private Quaternion _rotation;
        private Vector3 _velocity;
        private bool _isMoving;

        public int Tick
        {
            readonly get => _tick;
            set => _tick = value;
        }
        public Vector3 Position
        {
            readonly get => _position;
            set => _position = value;
        }
        public Quaternion Rotation
        {
            readonly get => _rotation;
            set => _rotation = value;
        }
        public Vector3 Velocity
        {
            readonly get => _velocity;
            set => _velocity = value;
        }
        public bool IsMoving
        {
            readonly get => _isMoving;
            set => _isMoving = value;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();

                reader.ReadValueSafe(out _tick);
                reader.ReadValueSafe(out _position);
                reader.ReadValueSafe(out _rotation);
                reader.ReadValueSafe(out _velocity);
                reader.ReadValueSafe(out _isMoving);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();

                writer.WriteValueSafe(_tick);
                writer.WriteValueSafe(_position);
                writer.WriteValueSafe(_rotation);
                writer.WriteValueSafe(_velocity);
                writer.WriteValueSafe(_isMoving);
            }
        }
    }
}