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
    [SerializeField] private bool serverAuthoritativeVelocity;
    [SerializeField] private bool serverAuthoritativeAngularVelocity;
    private NetworkVariable<Vector3> _networkAngularVelocity;

    private NetworkVariable<Vector3> _networkPosition;
    private NetworkVariable<Quaternion> _networkRotation;
    private NetworkVariable<Vector3> _networkVelocity;

    private Rigidbody _rigidbody;

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

    private void SyncNetworkVariable<T>(NetworkVariable<T> networkVariable, T value)
    {
        if (networkVariable.WritePerm == NetworkVariableWritePermission.Server)
        {
            if (IsServer)
                networkVariable.Value = value;
            else
                value = networkVariable.Value;
        }
        else
        {
            if (IsOwner)
                networkVariable.Value = value;
            else
                value = networkVariable.Value;
        }
    }
}