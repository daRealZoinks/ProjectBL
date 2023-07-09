using UnityEngine;

namespace LocalPlayer
{
    public abstract class HitScanWeapon : Weapon
    {
        [Tooltip("The range of the weapon.")] [SerializeField]
        protected float range = 1000f;

        public float Range
        {
            get => range;
            set => range = value;
        }
    }
}