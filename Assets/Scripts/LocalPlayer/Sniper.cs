using UnityEngine;

namespace LocalPlayer
{
    public class Sniper : HitScanWeapon
    {
        [Tooltip("The force of the sniper hit")] [SerializeField]
        private float force = 2500f;

        protected override void Shoot()
        {
            var firePointTransform = firePoint.transform;
            var ray = new Ray(firePointTransform.position, firePointTransform.forward);
            if (!Physics.Raycast(ray, out var hit, range)) return;

            var hitRigidbody = hit.rigidbody;
            if (hitRigidbody != null)
                hitRigidbody.AddForceAtPosition(firePointTransform.forward * force, hit.point, ForceMode.Impulse);
        }
    }
}