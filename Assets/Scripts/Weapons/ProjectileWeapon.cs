using UnityEngine;

namespace Weapons
{
    public abstract class ProjectileWeapon : Weapon
    {
        [Tooltip("The projectile to be fired.")]
        [SerializeField]
        protected Projectile projectilePrefab;

        [Tooltip("The speed at which the projectile will travel.")]
        [SerializeField]
        protected float projectileSpeed = 20f;

        protected override void Shoot()
        {
            var firePointTransform = firePoint.transform;
            var projectileInstance =
                Instantiate(projectilePrefab, firePointTransform.position, firePointTransform.rotation);
            projectileInstance.GetComponent<Rigidbody>().velocity = firePointTransform.forward * projectileSpeed;
        }
    }
}