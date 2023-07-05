using UnityEngine;
using UnityEngine.InputSystem;

namespace LocalPlayer
{
    [RequireComponent(typeof(PlayerCharacterController))]
    [RequireComponent(typeof(PlayerWallRunController))]
    public class InputProvider : MonoBehaviour
    {
        [Tooltip("The player camera controller")] [SerializeField]
        private PlayerCameraController playerCameraController;

        [Tooltip("The melee attack")] [SerializeField]
        private MeleeAttack meleeAttack;

        private PlayerCharacterController _playerCharacterController;
        private PlayerWallRunController _playerWallRunController;

        private void Awake()
        {
            _playerCharacterController = GetComponent<PlayerCharacterController>();
            _playerWallRunController = GetComponent<PlayerWallRunController>();
        }

        /// <summary>
        ///     Called when the player moves
        /// </summary>
        /// <param name="context"> The move input context </param>
        public void OnMove(InputAction.CallbackContext context)
        {
            _playerCharacterController.MovementInput = context.phase switch
            {
                InputActionPhase.Started => context.ReadValue<Vector2>(),
                InputActionPhase.Performed => context.ReadValue<Vector2>(),
                InputActionPhase.Canceled => Vector2.zero,
                _ => _playerCharacterController.MovementInput
            };
        }

        /// <summary>
        ///     Called when the player looks
        /// </summary>
        /// <param name="context"> The look input context </param>
        public void OnLook(InputAction.CallbackContext context)
        {
            playerCameraController.LookInput = context.phase switch
            {
                InputActionPhase.Started => context.ReadValue<Vector2>(),
                InputActionPhase.Performed => context.ReadValue<Vector2>(),
                InputActionPhase.Canceled => Vector2.zero,
                _ => playerCameraController.LookInput
            };
        }

        /// <summary>
        ///     Called when the player fires
        /// </summary>
        /// <param name="context"> The fire input context </param>
        public void OnFire(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    // TODO: Fire
                    break;
                case InputActionPhase.Performed:
                    // nothing
                    break;
                case InputActionPhase.Canceled:
                    // nothing
                    break;
            }
        }

        /// <summary>
        ///     Called when the player jumps
        /// </summary>
        /// <param name="context"> The jump input context </param>
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            _playerCharacterController.Jump();
            _playerWallRunController.WallJump();
        }

        /// <summary>
        ///     Called when the player sprints
        /// </summary>
        /// <param name="context"> The melee input context </param>
        public void OnMelee(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) meleeAttack.Attack();
        }
    }
}