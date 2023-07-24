using System;
using Unity.Netcode;
using UnityEngine;

namespace Weapons
{
    public class MeleeKick : NetworkBehaviour
    {
        [Tooltip("The player rigidbody.")]
        [SerializeField]
        private Rigidbody playerRigidbody;

        [Tooltip("The range of the kick.")]
        [SerializeField]
        [Range(0, 3f)]
        private float kickRange = 1f;

        [Tooltip("The radius of the kick.")]
        [SerializeField]
        [Range(0, 1f)]
        private float kickRadius = 0.75f;

        [Tooltip("The force of the kick.")]
        [SerializeField]
        [Range(0, 2000f)]
        private float kickForce = 1500f;

        /// <summary>
        ///     The range of the kick.
        /// </summary>
        public float KickRange => kickRange;

        private void OnDrawGizmos()
        {
            var cameraTransform = transform;
            var cameraTransformPosition = cameraTransform.position;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(cameraTransformPosition + cameraTransform.forward * kickRange, kickRadius);
        }

        /// <summary>
        ///     Hit all rigidbodies in the kick range.
        /// </summary>
        public void Attack()
        {
            if (IsOwner) AttackServerRpc();

            ExecuteAttack();
        }

        [ServerRpc]
        private void AttackServerRpc()
        {
            AttackClientRpc();
            ExecuteAttack();
        }

        [ClientRpc]
        private void AttackClientRpc()
        {
            if (!IsOwner) ExecuteAttack();
        }

        private void ExecuteAttack()
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