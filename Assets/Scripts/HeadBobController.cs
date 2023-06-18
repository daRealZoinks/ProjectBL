using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [SerializeField] private bool headBobEnabled = true;

    [Header("Bobbing")]
    [Tooltip("The amount of bobbing when the player is moving")]
    [SerializeField, Range(0, 0.1f)] private float amplitude = 0.08f;
    [Tooltip("The speed of the bobbing when the player is moving")]
    [SerializeField, Range(0, 30f)] private float frequency = 17.5f;

    [SerializeField] private PlayerCharacterController playerCharacterController;

    private Vector3 _startPosition;

    public bool HeadBobEnabled
    {
        get => headBobEnabled;
        set => headBobEnabled = value;
    }

    private void Awake()
    {
        _startPosition = transform.localPosition;
    }

    private void Update()
    {
        if (!headBobEnabled || !playerCharacterController.Grounded) return;

        if (playerCharacterController.Movement.magnitude > 0)
        {
            var speed = Mathf.Clamp01(playerCharacterController.Movement.magnitude / playerCharacterController.MaxSpeed);
            transform.localPosition = _startPosition + GetHeadBobPosition(speed);

            var targetPosition = playerCharacterController.transform.position;
            targetPosition.y += transform.localPosition.y;
            targetPosition += transform.forward * 2;

            transform.LookAt(targetPosition);
        }

        if (playerCharacterController.Stopping)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _startPosition, Time.deltaTime);
        }
    }

    public Vector3 GetHeadBobPosition(float speed)
    {
        var position = new Vector3(
            Mathf.Cos(Time.time * frequency / 2) * amplitude * 2,
            Mathf.Sin(Time.time * frequency) * amplitude,
            0);

        return position * speed;
    }
}