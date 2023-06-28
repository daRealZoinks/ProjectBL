using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MyNetworkRigidbody : NetworkBehaviour
{
    // Implementing lag compensation for a networked game object with complex movement or rotation can be more challenging than for a simple rigidbody.
    // Here are some general steps you can follow to implement lag compensation for a complex game object:

    // 1. Predict the future position and rotation of the game object on the client based on its current position,
    // velocity, and angular velocity. You can use the same approach as for a simple rigidbody, but you may need to
    // use more complex prediction algorithms to account for the game object's movement or rotation.

    // 2. When the server sends an update for the game object, interpolate the position and rotation of the game
    // object on the client towards the new values. You can use a variety of interpolation techniques, such as linear
    // interpolation or spline interpolation, to smoothly transition the game object to the new position and rotation.

    // 3. When the client receives an update for the game object, adjust the predicted position and rotation based on
    // the difference between the predicted values and the new values. You can use the same approach as for a simple
    // rigidbody, but you may need to use more complex adjustment algorithms to account for the game object's movement or rotation.

    // 4. To reduce the impact of latency on the game object's movement or rotation, you can use client-side prediction
    // to simulate the game object's movement or rotation on the client before receiving updates from the server. This
    // can help to reduce the perceived lag and improve the responsiveness of the game.

    // 5. To ensure that the game object's movement or rotation is consistent across all clients, you can use server
    // reconciliation to correct any discrepancies between the predicted values on the client and the actual values
    // on the server. This can help to ensure that all clients see the same game state and reduce the likelihood of desynchronization.

    // Implementing lag compensation for a complex game object can be a complex and challenging task, but following
    // these general steps can help to ensure that the game object's movement or rotation is smooth and consistent across all clients.

    private float _distance;
    private float _angle;

    [SerializeField] private bool syncronizeVelocity = true;
    [SerializeField] private bool syncronizeAngularVelocity = true;

    private Rigidbody _rigidbody;

    private readonly NetworkVariable<Vector3> _networkPosition = new(writePerm: NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<Quaternion> _networkRotation = new(writePerm: NetworkVariableWritePermission.Owner);

    private readonly NetworkVariable<Vector3> _networkVelocity = new(writePerm: NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<Vector3> _networkAngularVelocity = new(writePerm: NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            _rigidbody.position = Vector3.MoveTowards(_rigidbody.position, _networkPosition.Value, _distance * (1.0f / NetworkManager.Singleton.NetworkTickSystem.TickRate));
            _rigidbody.rotation = Quaternion.RotateTowards(_rigidbody.rotation, _networkRotation.Value, _angle * (1.0f / NetworkManager.Singleton.NetworkTickSystem.TickRate));
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            _networkPosition.Value = _rigidbody.position;
            _networkRotation.Value = _rigidbody.rotation;

            if (syncronizeVelocity)
            {
                _networkVelocity.Value = _rigidbody.velocity;
            }

            if (syncronizeAngularVelocity)
            {
                _networkAngularVelocity.Value = _rigidbody.angularVelocity;
            }
        }
        else
        {
            if (syncronizeVelocity || syncronizeAngularVelocity)
            {
                if (syncronizeVelocity)
                {
                    _rigidbody.velocity = _networkVelocity.Value;

                    _networkPosition.Value = _rigidbody.position;

                    _distance = Vector3.Distance(_rigidbody.position, _networkPosition.Value);
                }

                if (syncronizeAngularVelocity)
                {
                    _rigidbody.angularVelocity = _networkAngularVelocity.Value;

                    _networkRotation.Value = _rigidbody.rotation;

                    _angle = Quaternion.Angle(_rigidbody.rotation, _networkRotation.Value);
                }
            }
        }
    }
}
