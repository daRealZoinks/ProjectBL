using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private float _interpolationFactor = 0.1f;

    private NetworkVariable<PlayerState> _playerState = new();

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
        PlayerState playerState = new()
        {
            Position = transform.position,
            Rotation = transform.rotation,
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

    private Vector3 _lastPosition;
    private Vector3 _lastVelocity;
    private Vector3 _lastAngularVelocity;

    private void ConsumeState()
    {
        transform.position = Vector3.SmoothDamp(transform.position, _playerState.Value.Position, ref _lastPosition, _interpolationFactor);
        transform.rotation = Quaternion.Slerp(transform.rotation, _playerState.Value.Rotation, _interpolationFactor);
        _rigidbody.velocity = Vector3.SmoothDamp(_rigidbody.velocity, _playerState.Value.Velocity, ref _lastVelocity, _interpolationFactor);
        _rigidbody.angularVelocity = Vector3.SmoothDamp(_rigidbody.angularVelocity, _playerState.Value.AngularVelocity, ref _lastAngularVelocity, _interpolationFactor);
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
