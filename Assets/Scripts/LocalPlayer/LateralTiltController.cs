using UnityEngine;

namespace LocalPlayer
{
    public class LateralTiltController : MonoBehaviour
    {
        [SerializeField] private bool lateralTiltEnabled = true;

        [Header("Tilt")]
        [Tooltip("The angle of tilt when the player is moving")]
        [SerializeField]
        [Range(0, 10f)]
        private float angle = 2f;

        [Tooltip("The speed of the tilt when the player is moving")]
        [SerializeField]
        [Range(0, 15f)]
        private float speed = 5f;

        [SerializeField] private PlayerCharacterController playerCharacterController;

        private Vector3 _startRotation;

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
            if (!lateralTiltEnabled) return;

            var targetRotation = _startRotation;

            if (!playerCharacterController.Stopping && playerCharacterController.Grounded)
            {
                var directionOfMovement = transform.InverseTransformDirection(playerCharacterController.Movement);

                targetRotation.z -= directionOfMovement.normalized.x * angle;
            }
            else
            {
                targetRotation.z = 0;
            }

            var localRotation = transform.localRotation;

            targetRotation.x = localRotation.eulerAngles.x;
            targetRotation.y = localRotation.eulerAngles.y;

            transform.localRotation = Quaternion.Lerp(localRotation, Quaternion.Euler(targetRotation),
                speed * Time.deltaTime);
        }
    }
}