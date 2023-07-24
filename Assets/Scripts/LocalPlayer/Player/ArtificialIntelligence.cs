using UnityEngine;
using Weapons;

namespace LocalPlayer.Player
{
    /// <summary>
    ///     The artificial intelligence to use for moving towards the target.
    /// </summary>
    public class ArtificialIntelligence : MonoBehaviour
    {
        [Tooltip("The player camera controller to use for looking at the target.")]
        [SerializeField]
        private PlayerCameraController playerCameraController;

        [Tooltip("The melee attack to use when the target is close enough.")]
        [SerializeField]
        private MeleeKick meleeKick;

        private CharacterMovement _characterMovement;

        /// <summary>
        ///     The target to move towards.
        /// </summary>
        public Transform MoveTarget { get; set; }

        /// <summary>
        ///     The target to look at.
        /// </summary>
        public Transform LookTarget { get; set; }

        private void Awake()
        {
            _characterMovement = GetComponent<CharacterMovement>();
        }

        private void Update()
        {
            if (!MoveTarget) _characterMovement.MovementInput = Vector2.zero;
            if (!LookTarget) playerCameraController.LookInput = Vector2.zero;
            if (!MoveTarget || !LookTarget) return;

            var targetPosition = MoveTarget.position;

            _characterMovement.MovementInput = MoveTowards(targetPosition);
            playerCameraController.LookInput = LookAt(LookTarget.position);

            var distanceFromCharacterToTarget =
                (_characterMovement.Transform.position - targetPosition).magnitude;
            if (distanceFromCharacterToTarget < meleeKick.KickRange) meleeKick.Attack();
        }

        private void OnDrawGizmos()
        {
            if (MoveTarget != null)
            {
                Gizmos.color = Color.red;

                var targetPosition = MoveTarget.position;

                Gizmos.DrawSphere(targetPosition, 0.5f);
                Gizmos.DrawLine(transform.position, targetPosition);
            }

            if (LookTarget != null)
            {
                Gizmos.color = Color.blue;

                var targetPosition = LookTarget.position;

                Gizmos.DrawSphere(targetPosition, 0.5f);
                Gizmos.DrawLine(transform.position, targetPosition);
            }
        }

        private Vector2 MoveTowards(Vector3 target)
        {
            var artificialIntelligenceTransform = transform;

            var directionToTarget = target - artificialIntelligenceTransform.position;
            directionToTarget.y = 0f; // Ignore the vertical component

            var localDirection = Quaternion.Inverse(artificialIntelligenceTransform.rotation) * directionToTarget;

            Vector2 directionToTake = new(localDirection.x, localDirection.z);

            return directionToTake;
        }

        private Vector2 LookAt(Vector3 target)
        {
            var directionToTarget = target - transform.position;
            var targetRotation = Quaternion.LookRotation(directionToTarget);
            var characterRotation = Quaternion.LookRotation(transform.forward);

            var characterRotationDifference = targetRotation * Quaternion.Inverse(characterRotation);

            var characterEulerRotation = characterRotationDifference.eulerAngles;
            characterEulerRotation.x = Mathf.Repeat(characterEulerRotation.x + 180f, 360f) - 180f;
            characterEulerRotation.y = Mathf.Repeat(characterEulerRotation.y + 180f, 360f) - 180f;

            var desiredRotation = new Vector2(characterEulerRotation.y, 0f); // TODO: make the bot look up and down

            return desiredRotation;
        }
    }
}