using Unity.Netcode;
using UnityEngine;

/// <summary>
///     Allows for the use of <see cref="Rigidbody" /> on network objects.
///     By syncing the <see cref="Rigidbody" /> velocity and angular velocity over the network.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class AdvancedNetworkRigidbody : NetworkBehaviour
{
    [SerializeField] private bool serverAuthoritativePosition = true;
    [SerializeField] private bool serverAuthoritativeRotation = true;
    [SerializeField] private bool serverAuthoritativeVelocity = false;
    [SerializeField] private bool serverAuthoritativeAngularVelocity = false;

    private NetworkVariable<Vector3> _networkPosition;
    private NetworkVariable<Quaternion> _networkRotation;
    private NetworkVariable<Vector3> _networkVelocity;
    private NetworkVariable<Vector3> _networkAngularVelocity;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _networkPosition = new(writePerm: serverAuthoritativePosition ? NetworkVariableWritePermission.Server : NetworkVariableWritePermission.Owner);
        _networkRotation = new(writePerm: serverAuthoritativeRotation ? NetworkVariableWritePermission.Server : NetworkVariableWritePermission.Owner);
        _networkVelocity = new(writePerm: serverAuthoritativeVelocity ? NetworkVariableWritePermission.Server : NetworkVariableWritePermission.Owner);
        _networkAngularVelocity = new(writePerm: serverAuthoritativeAngularVelocity ? NetworkVariableWritePermission.Server : NetworkVariableWritePermission.Owner);
    }

    private void SyncNetworkVariable<T>(NetworkVariable<T> networkVariable, T value)
    {
        if (networkVariable.WritePerm == NetworkVariableWritePermission.Server)
        {
            if (IsServer)
            {
                networkVariable.Value = value;
            }
            else
            {
                value = networkVariable.Value;
            }
        }
        else
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

    // private void FixedUpdate()
    // {
    //     if (_networkPosition.WritePerm == NetworkVariableWritePermission.Server)
    //     {
    //         if (IsServer)
    //         {
    //             _networkPosition.Value = _rigidbody.position;
    //         }
    //         else
    //         {
    //             _rigidbody.position = _networkPosition.Value;
    //         }
    //     }
    //     else
    //     {
    //         if (IsOwner)
    //         {
    //             _networkPosition.Value = _rigidbody.position;
    //         }
    //         else
    //         {
    //             _rigidbody.position = _networkPosition.Value;
    //         }
    //     }

    //     if (_networkRotation.WritePerm == NetworkVariableWritePermission.Server)
    //     {
    //         if (IsServer)
    //         {
    //             _networkRotation.Value = _rigidbody.rotation;
    //         }
    //         else
    //         {
    //             _rigidbody.rotation = _networkRotation.Value;
    //         }
    //     }
    //     else
    //     {
    //         if (IsOwner)
    //         {
    //             _networkRotation.Value = _rigidbody.rotation;
    //         }
    //         else
    //         {
    //             _rigidbody.rotation = _networkRotation.Value;
    //         }
    //     }

    //     if (_networkVelocity.WritePerm == NetworkVariableWritePermission.Server)
    //     {
    //         if (IsServer)
    //         {
    //             _networkVelocity.Value = _rigidbody.velocity;
    //         }
    //         else
    //         {
    //             _rigidbody.velocity = _networkVelocity.Value;
    //         }
    //     }
    //     else
    //     {
    //         if (IsOwner)
    //         {
    //             _networkVelocity.Value = _rigidbody.velocity;
    //         }
    //         else
    //         {
    //             _rigidbody.velocity = _networkVelocity.Value;
    //         }
    //     }

    //     if (_networkAngularVelocity.WritePerm == NetworkVariableWritePermission.Server)
    //     {
    //         if (IsServer)
    //         {
    //             _networkAngularVelocity.Value = _rigidbody.angularVelocity;
    //         }
    //         else
    //         {
    //             _rigidbody.angularVelocity = _networkAngularVelocity.Value;
    //         }
    //     }
    //     else
    //     {
    //         if (IsOwner)
    //         {
    //             _networkAngularVelocity.Value = _rigidbody.angularVelocity;
    //         }
    //         else
    //         {
    //             _rigidbody.angularVelocity = _networkAngularVelocity.Value;
    //         }
    //     }
    // }

    private void FixedUpdate()
    {
        // SyncNetworkVariable(_networkPosition, _rigidbody.position);
        // SyncNetworkVariable(_networkRotation, _rigidbody.rotation);
        SyncNetworkVariable(_networkVelocity, _rigidbody.velocity);
        SyncNetworkVariable(_networkAngularVelocity, _rigidbody.angularVelocity);
    }
}
