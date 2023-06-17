using Cinemachine;
using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [SerializeField] private bool headBobEnabled = true;

    [Header("Bobbing")]
    [Tooltip("The amount of bobbing when the player is moving")]
    [SerializeField, Range(0, 0.1f)] private float amplitude = 0.015f;
    [Tooltip("The speed of the bobbing when the player is moving")]
    [SerializeField, Range(0, 30f)] private float frequency = 10f;

    [SerializeField] private CinemachineVirtualCamera _camera = null;

    private float _toggleSpeed = 3f;
    private Vector3 _startPosition;
    private PlayerCharacterController _playerCharacterController;

    private void Awake()
    {
        _playerCharacterController = GetComponent<PlayerCharacterController>();
        _startPosition = _camera.transform.localPosition;
    }

    private void Update()
    {
        if (!headBobEnabled) return;
        CheckMotion();
        ResetPosition();
        _camera.transform.LookAt(FocusTarget());
    }

    private void PlayMotion(Vector3 motion)
    {
        _camera.transform.localPosition = _startPosition + motion;
    }

    private void CheckMotion()
    {
        float speed = _playerCharacterController.Movement.magnitude;

        if (speed < _toggleSpeed) return;
        if (!_playerCharacterController.Grounded) return;

        PlayMotion(FootStepMotion());
    }

    private Vector3 FootStepMotion()
    {
        Vector3 position = Vector3.zero;
        position.y = Mathf.Sin(Time.time * frequency) * amplitude;
        position.x = Mathf.Cos(Time.time * frequency / 2) * amplitude * 2;
        return position;
    }

    private void ResetPosition()
    {
        if (_camera.transform.localPosition == _startPosition) return;
        _camera.transform.localPosition = Vector3.Lerp(_camera.transform.localPosition, _startPosition, Time.deltaTime);
    }

    private Vector3 FocusTarget()
    {
        Vector3 position = new Vector3(transform.position.x, transform.position.y + _camera.transform.localPosition.y, transform.position.z);
        position += _camera.transform.forward * 2;
        return position;
    }
}