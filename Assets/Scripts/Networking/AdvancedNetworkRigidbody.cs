using System;
using Unity.Netcode;
using UnityEngine;

namespace Networking
{
    /// <summary>
    ///     Allows for the use of <see cref="Rigidbody" /> on network objects.
    ///     By syncing the <see cref="Rigidbody" /> velocity and angular velocity over the network.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class AdvancedNetworkRigidbody : NetworkBehaviour
    {
        [SerializeField] private bool serverAuthoritativePosition = true;
        [SerializeField] private bool serverAuthoritativeRotation = true;
        [SerializeField] private bool serverAuthoritativeVelocity;
        [SerializeField] private bool serverAuthoritativeAngularVelocity;
        
        private Rigidbody _rigidbody;

        private NetworkVariable<Vector3> _networkPosition;
        private NetworkVariable<Quaternion> _networkRotation;
        private NetworkVariable<Vector3> _networkVelocity;
        private NetworkVariable<Vector3> _networkAngularVelocity;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            _networkPosition = new NetworkVariable<Vector3>(writePerm: serverAuthoritativePosition
                ? NetworkVariableWritePermission.Server
                : NetworkVariableWritePermission.Owner);
            _networkRotation = new NetworkVariable<Quaternion>(writePerm: serverAuthoritativeRotation
                ? NetworkVariableWritePermission.Server
                : NetworkVariableWritePermission.Owner);
            _networkVelocity = new NetworkVariable<Vector3>(writePerm: serverAuthoritativeVelocity
                ? NetworkVariableWritePermission.Server
                : NetworkVariableWritePermission.Owner);
            _networkAngularVelocity = new NetworkVariable<Vector3>(writePerm: serverAuthoritativeAngularVelocity
                ? NetworkVariableWritePermission.Server
                : NetworkVariableWritePermission.Owner);
        }
        
        private void FixedUpdate()
        {
            SyncNetworkVariable(_networkPosition, _rigidbody.position);
            SyncNetworkVariable(_networkRotation, _rigidbody.rotation);
            SyncNetworkVariable(_networkVelocity, _rigidbody.velocity);
            SyncNetworkVariable(_networkAngularVelocity, _rigidbody.angularVelocity);
        }

        private void SyncNetworkVariable<T>(NetworkVariable<T> networkVariable, T value)
        {
            switch (networkVariable.WritePerm)
            {
                case NetworkVariableWritePermission.Server when IsServer:
                    networkVariable.Value = value;
                    break;
                case NetworkVariableWritePermission.Server when IsOwner:
                    value = networkVariable.Value;
                    break;
                case NetworkVariableWritePermission.Owner when IsOwner:
                    networkVariable.Value = value;
                    break;
                case NetworkVariableWritePermission.Owner when IsServer:
                    value = networkVariable.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}