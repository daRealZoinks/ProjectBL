using UnityEngine;

namespace LocalPlayer
{
    public class MeleeKick : MonoBehaviour
    {
        [Tooltip("The player rigidbody.")] [SerializeField]
        private Rigidbody playerRigidbody;

        [Tooltip("The range of the kick.")] [SerializeField] [Range(0, 3f)]
        private float kickRange = 1f;

        [Tooltip("The radius of the kick.")] [SerializeField] [Range(0, 1f)]
        private float kickRadius = 0.75f;

        [Tooltip("The force of the kick.")] [SerializeField] [Range(0, 2000f)]
        private float kickForce = 1500f;

        /// <summary>
        ///     The range of the kick.
        /// </summary>
        public float KickRange => kickRange;

        /// <summary>
        ///     Hit all rigidbodies in the kick range.
        /// </summary>
        public void Attack()
        {
            var cameraTransform = transform;
            var cameraTransformPosition = cameraTransform.position;

            var hits = Physics.OverlapCapsule(cameraTransformPosition,
                cameraTransformPosition + cameraTransform.forward * kickRange, kickRadius);

            foreach (var hit in hits)
                if (hit.TryGetComponent<Rigidbody>(out var rigidbodyComponent))
                {
                    if (rigidbodyComponent == playerRigidbody) continue;
                    rigidbodyComponent.AddForce(transform.forward * kickForce, ForceMode.Impulse);
                }
        }
    }
}