using UnityEngine;
using UnityEngine.InputSystem;
using Weapons;

namespace LocalPlayer.Player
{
    [RequireComponent(typeof(CharacterMovement))]
    public class InputProvider : MonoBehaviour
    {
        [Tooltip("The melee attack")]
        [SerializeField]
        private MeleeKick meleeKick;

        [Tooltip("The weapon manager")]
        [SerializeField]
        private WeaponManager weaponManager;

        private CharacterMovement _characterMovement;

        private void Awake()
        {
            _characterMovement = GetComponent<CharacterMovement>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var value = context.phase switch
            {
                InputActionPhase.Started or InputActionPhase.Performed => context.ReadValue<Vector2>(),
                _ => Vector2.zero
            };

            _characterMovement.MovementInput = value;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            _characterMovement.Jump();
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            weaponManager.Shoot();
        }

        public void OnMelee(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            meleeKick.Attack();
        }
    }
}