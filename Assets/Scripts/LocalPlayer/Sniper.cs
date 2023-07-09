using UnityEngine;

namespace LocalPlayer
{
    public class Sniper : HitScanWeapon
    {
        [Tooltip("The force of the weapon.")] [SerializeField]
        private float force = 10f;

        protected override void Shoot()
        {
            var ray = new Ray(firePoint.position, firePoint.forward);

            if (!Physics.Raycast(ray, out var hit, range)) return;

            var hitRigidbody = hit.rigidbody;
            if (hitRigidbody != null)
                hitRigidbody.AddForceAtPosition(firePoint.forward * force, hit.point, ForceMode.Impulse);
        }
    }
}