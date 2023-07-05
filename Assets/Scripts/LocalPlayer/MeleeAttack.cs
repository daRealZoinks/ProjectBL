using UnityEngine;

namespace LocalPlayer
{
    public class MeleeAttack : MonoBehaviour
    {
        [Tooltip("The range of the attack.")] [SerializeField] [Range(0, 5f)]
        private float attackRange = 3f;

        [Tooltip("The radius of the attack.")] [SerializeField] [Range(0, 1f)]
        private float attackRadius = 0.75f;

        [Tooltip("The force of the attack.")] [SerializeField] [Range(0, 1000f)]
        private float attackForce = 250f;

        /// <summary>
        ///     The range of the attack.
        /// </summary>
        public float AttackRange => attackRange;

        /// <summary>
        ///     Hit all rigidbodies in the attack range.
        /// </summary>
        public void Attack()
        {
            var cameraTransform = transform;
            var cameraTransformPosition = cameraTransform.position;

            var hits = new Collider[5];
            var numHits = Physics.OverlapCapsuleNonAlloc(cameraTransformPosition,
                cameraTransformPosition + cameraTransform.forward * attackRange, attackRadius, hits);

            for (var i = 0; i < numHits; i++)
                if (hits[i].TryGetComponent<Rigidbody>(out var rigidbodyComponent))
                    rigidbodyComponent.AddForce(transform.forward * attackForce, ForceMode.Impulse);
        }
    }
}