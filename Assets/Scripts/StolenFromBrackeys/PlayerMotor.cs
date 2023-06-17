using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    public Vector3 Velocity { private get; set; } = Vector3.zero;
    public Vector3 Rotation { private get; set; } = Vector3.zero;
    public Vector3 CameraRotation { private get; set; } = Vector3.zero;
    public Vector3 ThrusterForce { private get; set; } = Vector3.zero;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (Velocity != Vector3.zero)
        {
            _rb.MovePosition(_rb.position + Velocity * Time.fixedDeltaTime);
        }

        _rb.MoveRotation(_rb.rotation * Quaternion.Euler(Rotation));
        virtualCamera.transform.Rotate(-CameraRotation);
        if (ThrusterForce != Vector3.zero)
        {
            _rb.AddForce(ThrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }
}