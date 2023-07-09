using UnityEngine;

namespace LocalPlayer
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
            var firepointTransform = firePoint.transform;
            var projectileInstance = Instantiate(projectilePrefab, firepointTransform.position, firepointTransform.rotation);
            projectileInstance.GetComponent<Rigidbody>().velocity = firepointTransform.forward * projectileSpeed;
        }
    }
}