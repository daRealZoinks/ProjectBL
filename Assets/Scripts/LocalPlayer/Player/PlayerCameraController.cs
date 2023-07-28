using UnityEngine;

namespace LocalPlayer.Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [Tooltip("The character movement")]
        [SerializeField]
        private CharacterMovement characterMovement;

        [Space]
        [Header("Camera Rotation Constraints")]
        [Tooltip("The minimum angle of the camera")]
        [SerializeField]
        private float minimumAngle = -90f;

        [Tooltip("The maximum angle of the camera")]
        [SerializeField]
        private float maximumAngle = 90f;

        private float _xRotation;

        /// <summary>
        ///     The input of the camera
        /// </summary>
        public Vector2 LookInput { get; set; }

        private void Update()
        {
            LookInput = characterMovement.LookInput;

            _xRotation -= LookInput.y;
            _xRotation = Mathf.Clamp(_xRotation, minimumAngle, maximumAngle);

            var localRotation = transform.localRotation;

            localRotation = Quaternion.Euler(_xRotation, localRotation.eulerAngles.y, localRotation.eulerAngles.z);
            transform.localRotation = localRotation;
        }

        private void OnDrawGizmos()
        {
            var cameraTransform = transform;
            var cameraTransformPosition = cameraTransform.position;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(cameraTransformPosition, cameraTransformPosition + cameraTransform.forward * 5f);
        }
    }
}