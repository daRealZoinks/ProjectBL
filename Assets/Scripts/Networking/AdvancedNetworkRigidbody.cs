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
        private Rigidbody _rigidbody;

        private NetworkVariable<Vector3> _networkVelocity;
        private NetworkVariable<Vector3> _networkAngularVelocity;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            _networkVelocity = new NetworkVariable<Vector3>(writePerm: NetworkVariableWritePermission.Owner);
            _networkAngularVelocity = new NetworkVariable<Vector3>(writePerm: NetworkVariableWritePermission.Owner);
        }

        private void FixedUpdate()
        {
            SyncNetworkVariable(_networkVelocity, _rigidbody.velocity);
            SyncNetworkVariable(_networkAngularVelocity, _rigidbody.angularVelocity);
        }

        private void SyncNetworkVariable<T>(NetworkVariable<T> networkVariable, T value)
        {
            if (IsOwner)
            {
                networkVariable.Value = value;
            }
            else
            {
                value = networkVariable.Value;
            }
        }
    }
}