using Unity.Netcode;
using UnityEngine;

namespace Game.Networking
{
    /// <summary>
    ///     Allows for the use of <see cref="Rigidbody" /> on network objects.
    ///     By syncing the <see cref="Rigidbody" /> velocity and angular velocity over the network.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class AdvancedNetworkRigidbody : NetworkBehaviour
    {
        [SerializeField]
        private float InterpolationTime = 0.1f;

        private Rigidbody _rigidbody;
        private readonly NetworkVariable<AdvancedNetworkRigidbodyState> _networkState = new(writePerm: NetworkVariableWritePermission.Owner);

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private float _lastStateTime;

        private AdvancedNetworkRigidbodyState _lastState;

        private void FixedUpdate()
        {
            if (IsOwner)
            {
                _networkState.Value = new()
                {
                    Position = _rigidbody.position,
                    Rotation = _rigidbody.rotation.eulerAngles,
                    Velocity = _rigidbody.velocity,
                    AngularVelocity = _rigidbody.angularVelocity
                };
            }
            else
            {
                float timeSinceLastState = Time.time - _lastStateTime;
                float lerpT = Mathf.Clamp01(timeSinceLastState / InterpolationTime);

                _rigidbody.position = Vector3.Lerp(_lastState.Position, _networkState.Value.Position, lerpT);
                _rigidbody.velocity = Vector3.Lerp(_lastState.Velocity, _networkState.Value.Velocity, lerpT);
                _rigidbody.angularVelocity = Vector3.Lerp(_lastState.AngularVelocity, _networkState.Value.AngularVelocity, lerpT);

                Quaternion lastRotation = Quaternion.Euler(_lastState.Rotation);
                Quaternion targetRotation = Quaternion.Euler(_networkState.Value.Rotation);
                Quaternion slerpRotation = Quaternion.Slerp(lastRotation, targetRotation, lerpT);
                _rigidbody.rotation = slerpRotation;

                _lastStateTime = Time.time;
                _lastState = _networkState.Value;
            }
        }

        public struct AdvancedNetworkRigidbodyState : INetworkSerializable
        {
            private Vector3 _position;
            private Vector3 _rotation;
            private Vector3 _velocity;
            private Vector3 _angularVelocity;

            public Vector3 Position
            {
                readonly get => _position;
                set => _position = value;
            }
            public Vector3 Rotation
            {
                readonly get => _rotation;
                set => _rotation = value;
            }
            public Vector3 Velocity
            {
                readonly get => _velocity;
                set => _velocity = value;
            }
            public Vector3 AngularVelocity
            {
                readonly get => _angularVelocity;
                set => _angularVelocity = value;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                if (serializer.IsWriter)
                {
                    var writer = serializer.GetFastBufferWriter();

                    writer.WriteValueSafe(_position);
                    writer.WriteValueSafe(_rotation);
                    writer.WriteValueSafe(_velocity);
                    writer.WriteValueSafe(_angularVelocity);
                }
                else
                {
                    var reader = serializer.GetFastBufferReader();

                    reader.ReadValueSafe(out _position);
                    reader.ReadValueSafe(out _rotation);
                    reader.ReadValueSafe(out _velocity);
                    reader.ReadValueSafe(out _angularVelocity);
                }
            }
        }
    }
}