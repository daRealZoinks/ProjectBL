using LocalPlayer.Player;
using UnityEngine;

namespace LocalPlayer.ProceduralAnimation
{
    public class LateralTiltController : MonoBehaviour
    {
        [Tooltip("Whether or not the lateral tilt is enabled")]
        [SerializeField]
        private bool lateralTiltEnabled = true;

        [Header("Tilt")]
        [Tooltip("The angle of tilt when the player is moving")]
        [SerializeField]
        [Range(0, 10f)]
        private float angle = 2f;

        [Tooltip("The angle of tilt when the player is wall running")]
        [SerializeField]
        [Range(0, 10f)]
        private float wallRunAngle = 5f;

        [Tooltip("The speed of the tilt when the player is moving")]
        [SerializeField]
        [Range(0, 15f)]
        private float speed = 5f;

        [Tooltip("The character movement")]
        [SerializeField]
        private CharacterMovement characterMovement;

        [Tooltip("The player wall run controller")]
        [SerializeField]
        private PlayerWallRunController playerWallRunController;

        private Vector3 _startRotation;

        /// <summary>
        ///     Whether or not the lateral tilt is enabled.
        /// </summary>
        public bool LateralTiltEnabled
        {
            get => lateralTiltEnabled;
            set => lateralTiltEnabled = value;
        }

        private void Awake()
        {
            _startRotation = transform.localRotation.eulerAngles;
        }

        private void Update()
        {
            if (!LateralTiltEnabled) return;

            var targetRotation = _startRotation;

            if (!characterMovement.Stopping && characterMovement.IsGrounded)
            {
                var directionOfMovement = transform.InverseTransformDirection(characterMovement.Velocity);

                targetRotation.z -= directionOfMovement.normalized.x * angle;
            }
            else
            {
                targetRotation.z = 0;
            }

            if (playerWallRunController.IsWallRunning)
            {
                targetRotation.z = 0;

                if (playerWallRunController.IsWallRight)
                {
                    targetRotation.z = wallRunAngle;
                }

                if (playerWallRunController.IsWallLeft)
                {
                    targetRotation.z = -wallRunAngle;
                }
            }

            var localRotation = transform.localRotation;

            targetRotation.x = localRotation.eulerAngles.x;
            targetRotation.y = localRotation.eulerAngles.y;

            transform.localRotation = Quaternion.Lerp(localRotation, Quaternion.Euler(targetRotation),
                speed * Time.deltaTime);
        }
    }
}