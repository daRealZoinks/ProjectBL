using UnityEngine;

namespace LocalPlayer
{
    public class HeadBobController : MonoBehaviour
    {
        [Tooltip("Whether or not head bob is enabled")] [SerializeField]
        private bool headBobEnabled = true;

        [Header("Bobbing")]
        [Tooltip("The amount of bobbing when the player is moving")]
        [SerializeField]
        [Range(0, 0.1f)]
        private float amplitude = 0.08f;

        [Tooltip("The speed of the bobbing when the player is moving")] [SerializeField] [Range(0, 30f)]
        private float frequency = 18.5f;

        [Tooltip("The player character controller")] [SerializeField]
        private PlayerCharacterController playerCharacterController;

        [Tooltip("The player wall run controller")] [SerializeField]
        private PlayerWallRunController playerWallRunController;

        private Vector3 _startPosition;
        private Vector3 _targetLookAtPosition;

        /// <summary>
        ///     Whether or not head bob is enabled
        /// </summary>
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
            if (!HeadBobEnabled) return;

            if ((!playerCharacterController.IsGrounded || playerCharacterController.Stopping) &&
                !playerWallRunController.IsWallRunning)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, _startPosition, Time.deltaTime);
                return;
            }

            if (!(playerCharacterController.Velocity.magnitude > 0) && !playerWallRunController.IsWallRunning) return;

            var speed = Mathf.Clamp01(playerCharacterController.Velocity.magnitude /
                                      playerCharacterController.MaxSpeed);
            transform.localPosition = _startPosition + GetHeadBobPosition(speed);
        }

        private Vector3 GetHeadBobPosition(float speed)
        {
            var position = new Vector3(
                Mathf.Cos(Time.time * frequency / 2) * amplitude * 2,
                Mathf.Sin(Time.time * frequency) * amplitude,
                0);

            return position * speed;
        }
    }
}