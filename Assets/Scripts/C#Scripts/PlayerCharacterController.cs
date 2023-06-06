using Cinemachine;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;

    public Vector2 Direction { get; set; }
    public Vector2 Look { get; set; }

    private Rigidbody _rigidbody;
    private float _xRotation = 0f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        var finalDirection = Direction.x * transform.right + transform.forward * Direction.y;

        _rigidbody.MovePosition(_rigidbody.position + speed * Time.deltaTime * finalDirection);

        _xRotation -= Look.y * sensitivity;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        _cinemachineVirtualCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        _rigidbody.MoveRotation(transform.rotation * Quaternion.Euler(0f, Look.x * sensitivity, 0f));
    }
}
