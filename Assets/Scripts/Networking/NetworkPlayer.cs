using Unity.Netcode;
using UnityEngine;

namespace Networking
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [SerializeField] private float interpolationFactor = 0.1f;

        private readonly NetworkVariable<PlayerState> _playerState = new();

        private Vector3 _lastAngularVelocity;
        private Vector3 _lastPosition;
        private Vector3 _lastVelocity;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (IsOwner) TransmitState();
            else ConsumeState();
        }

        private void TransmitState()
        {
            var playerTransform = transform;

            PlayerState playerState = new()
            {
                Position = playerTransform.position,
                Rotation = playerTransform.rotation,
                Velocity = _rigidbody.velocity,
                AngularVelocity = _rigidbody.angularVelocity
            };

            TransmitStateServerRpc(playerState);
        }

        [ServerRpc]
        private void TransmitStateServerRpc(PlayerState playerState)
        {
            _playerState.Value = playerState;
        }

        private void ConsumeState()
        {
            var playerTransform = transform;

            playerTransform.position = Vector3.SmoothDamp(playerTransform.position, _playerState.Value.Position,
                ref _lastPosition,
                interpolationFactor);
            transform.rotation =
                Quaternion.Slerp(playerTransform.rotation, _playerState.Value.Rotation, interpolationFactor);
            _rigidbody.velocity = Vector3.SmoothDamp(_rigidbody.velocity, _playerState.Value.Velocity,
                ref _lastVelocity,
                interpolationFactor);
            _rigidbody.angularVelocity = Vector3.SmoothDamp(_rigidbody.angularVelocity,
                _playerState.Value.AngularVelocity,
                ref _lastAngularVelocity, interpolationFactor);
        }

        private struct PlayerState : INetworkSerializable
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Velocity;
            public Vector3 AngularVelocity;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref Position);
                serializer.SerializeValue(ref Rotation);
                serializer.SerializeValue(ref Velocity);
                serializer.SerializeValue(ref AngularVelocity);
            }
        }
    }
}