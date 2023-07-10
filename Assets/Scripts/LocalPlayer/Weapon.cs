using UnityEngine;

namespace LocalPlayer
{
    public abstract class Weapon : MonoBehaviour
    {
        [Tooltip("The point from which the weapon will fire.")] [SerializeField]
        protected Firepoint firePoint;

        [Tooltip("The amount of ammo the weapon has.")] [SerializeField]
        protected int ammo = 6;

        [Tooltip("The amount of time between shots.")] [SerializeField]
        protected float coolDown = 1f;

        private bool _canShoot = true;
        private int _currentAmmo;

        protected virtual void Start()
        {
            _currentAmmo = ammo;
        }

        private void OnDrawGizmos()
        {
            var firePointTransform = firePoint.transform;
            var firePointPosition = firePointTransform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(firePointPosition, firePointPosition + firePointTransform.forward * 5f);
        }

        public void ExecuteShoot()
        {
            if (!_canShoot) return;

            Shoot();

            _canShoot = false;
            Invoke(nameof(ResetCanShoot), coolDown);
            _currentAmmo--;
            if (_currentAmmo <= 0)
                // TODO: Take weapon away from player.
                Debug.Log("Out of ammo!");
        }

        protected abstract void Shoot();

        private void ResetCanShoot()
        {
            _canShoot = true;
        }
    }
}