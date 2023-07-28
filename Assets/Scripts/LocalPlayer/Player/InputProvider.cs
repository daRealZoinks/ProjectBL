using UnityEngine;
using UnityEngine.InputSystem;
using Weapons;

namespace LocalPlayer.Player
{
    [RequireComponent(typeof(CharacterMovement))]
    public class InputProvider : MonoBehaviour
    {
        [Tooltip("The player camera controller")]
        [SerializeField]
        private PlayerCameraController playerCameraController;

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


        /// <summary>
        ///     Called when the player moves
        /// </summary>
        /// <param name="context"> The move input context </param>
        public void OnMove(InputAction.CallbackContext context)
        {
            var value = context.phase switch
            {
                InputActionPhase.Started or InputActionPhase.Performed => context.ReadValue<Vector2>(),
                _ => Vector2.zero
            };

            _characterMovement.MovementInput = value;
        }


        /// <summary>
        ///     Called when the player looks
        /// </summary>
        /// <param name="context"> The look input context </param>
        public void OnLook(InputAction.CallbackContext context)
        {
            var value = context.phase switch
            {
                InputActionPhase.Started or InputActionPhase.Performed => context.ReadValue<Vector2>(),
                _ => Vector2.zero
            };

            _characterMovement.LookInput = value;
            // playerCameraController.LookInput = value;
        }


        /// <summary>
        ///     Called when the player jumps
        /// </summary>
        /// <param name="context"> The jump input context </param>
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            _characterMovement.Jump();
        }


        /// <summary>
        ///     Called when the player fires
        /// </summary>
        /// <param name="context"> The fire input context </param>
        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            weaponManager.Shoot();
        }


        /// <summary>
        ///     Called when the player sprints
        /// </summary>
        /// <param name="context"> The melee input context </param>
        public void OnMelee(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            meleeKick.Attack();
        }
    }
}