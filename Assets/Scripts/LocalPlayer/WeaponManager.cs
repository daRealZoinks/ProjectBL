using System.Collections.Generic;
using UnityEngine;

namespace LocalPlayer
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private List<Weapon> weapons;
        [SerializeField] private Weapon activeWeapon;

        public Weapon ActiveWeapon
        {
            get => activeWeapon;
            private set
            {
                activeWeapon = value;
                foreach (var weapon in weapons)
                {
                    weapon.gameObject.SetActive(weapon == activeWeapon);
                }
            }
        }

        private void Start()
        {
            ActiveWeapon = activeWeapon;
        }

        public void Shoot()
        {
            ActiveWeapon.ExecuteShoot();
        }
    }
}