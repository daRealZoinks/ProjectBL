using UnityEngine;

namespace LocalPlayer
{
    public abstract class ProjectileWeapon : Weapon
    {
        [Tooltip("The projectile to be fired.")] [SerializeField]
        protected GameObject projectilePrefab;

        [Tooltip("The speed at which the projectile will travel.")] [SerializeField]
        protected float projectileSpeed = 20f;

        protected override void Shoot()
        {
            var projectileInstance = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            projectileInstance.GetComponent<Rigidbody>().velocity = firePoint.forward * projectileSpeed;
        }
    }
}