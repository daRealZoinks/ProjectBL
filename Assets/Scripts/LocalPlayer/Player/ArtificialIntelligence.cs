using UnityEngine;
using Weapons;

namespace LocalPlayer.Player
{
    /// <summary>
    ///     The artificial intelligence to use for moving towards the target.
    /// </summary>
    public class ArtificialIntelligence : MonoBehaviour
    {
        [Tooltip("The melee attack to use when the target is close enough.")]
        [SerializeField]
        private MeleeKick meleeKick;

        public Transform MoveTarget { get; set; }
        public Transform LookTarget { get; set; }

        private CharacterMovement _characterMovement;

        private void Awake()
        {
            _characterMovement = GetComponent<CharacterMovement>();
        }

        private void Update()
        {
            if (!MoveTarget) _characterMovement.MovementInput = Vector2.zero;
            if (!MoveTarget || !LookTarget) return;

            var targetPosition = MoveTarget.position;

            _characterMovement.MovementInput = MoveTowards(targetPosition);
            _characterMovement.CinemachineVirtualCamera.LookAt = LookTarget;

            var distanceFromCharacterToTarget =
                (_characterMovement.transform.position - targetPosition).magnitude;
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
    }
}