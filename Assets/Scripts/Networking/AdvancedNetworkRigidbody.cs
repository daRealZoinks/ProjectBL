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
        private readonly NetworkVariable<Vector3> _networkAngularVelocity =
            new(writePerm: NetworkVariableWritePermission.Owner);

        private readonly NetworkVariable<Vector3> _networkVelocity =
            new(writePerm: NetworkVariableWritePermission.Owner);

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (IsOwner)
            {
                _networkVelocity.Value = _rigidbody.velocity;
                _networkAngularVelocity.Value = _rigidbody.angularVelocity;
            }
            else
            {
                _rigidbody.velocity = _networkVelocity.Value;
                _rigidbody.angularVelocity = _networkAngularVelocity.Value;
            }
        }
    }
}