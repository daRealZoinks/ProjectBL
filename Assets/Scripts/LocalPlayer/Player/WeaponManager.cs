using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Weapons;

namespace LocalPlayer.Player
{
    public class WeaponManager : NetworkBehaviour
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
            if (IsOwner) ShootServerRpc();

            ActiveWeapon.ExecuteShoot();
        }

        [ServerRpc]
        private void ShootServerRpc()
        {
            ShootClientRpc();
            ActiveWeapon.ExecuteShoot();
        }

        [ClientRpc]
        private void ShootClientRpc()
        {
            if (!IsOwner) ActiveWeapon.ExecuteShoot();
        }
    }
}