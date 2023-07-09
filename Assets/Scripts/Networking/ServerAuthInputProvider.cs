using LocalPlayer;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Networking
{
    [RequireComponent(typeof(PlayerCharacterController))]
    [RequireComponent(typeof(PlayerWallRunController))]
    public class ServerAuthInputProvider : NetworkBehaviour
    {
        [Tooltip("The player camera controller")] [SerializeField]
        private PlayerCameraController playerCameraController;

        [Tooltip("The melee attack")] [SerializeField]
        private MeleeKick meleeKick;

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
            var value = context.phase switch
            {
                InputActionPhase.Started => context.ReadValue<Vector2>(),
                InputActionPhase.Performed => context.ReadValue<Vector2>(),
                InputActionPhase.Canceled => Vector2.zero,
                _ => Vector2.zero
            };

            if (IsOwner) OnMoveServerRpc(value);

            Move(value);
        }

        [ServerRpc]
        private void OnMoveServerRpc(Vector2 value)
        {
            OnMoveClientRpc(value);
        }

        [ClientRpc]
        private void OnMoveClientRpc(Vector2 value)
        {
            if (!IsOwner) Move(value);
        }

        private void Move(Vector2 value)
        {
            _playerCharacterController.MovementInput = value;
        }

        /// <summary>
        ///     Called when the player looks
        /// </summary>
        /// <param name="context"> The look input context </param>
        public void OnLook(InputAction.CallbackContext context)
        {
            var value = context.phase switch
            {
                InputActionPhase.Started => context.ReadValue<Vector2>(),
                InputActionPhase.Performed => context.ReadValue<Vector2>(),
                InputActionPhase.Canceled => Vector2.zero,
                _ => Vector2.zero
            };

            if (IsOwner) OnLookServerRpc(value);

            Look(value);
        }

        [ServerRpc]
        private void OnLookServerRpc(Vector2 value)
        {
            OnLookClientRpc(value);
        }

        [ClientRpc]
        private void OnLookClientRpc(Vector2 value)
        {
            if (!IsOwner) Look(value);
        }

        private void Look(Vector2 value)
        {
            playerCameraController.LookInput = value;
        }

        /// <summary>
        ///     Called when the player fires
        /// </summary>
        /// <param name="context"> The fire input context </param>
        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            if (IsOwner) OnFireServerRpc();
            Fire();
        }

        [ServerRpc]
        private void OnFireServerRpc()
        {
            OnFireClientRpc();
        }

        [ClientRpc]
        private void OnFireClientRpc()
        {
            if (!IsOwner) Fire();
        }

        private void Fire()
        {
            // TODO: Fire
        }

        /// <summary>
        ///     Called when the player jumps
        /// </summary>
        /// <param name="context"> The jump input context </param>
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            if (IsOwner) OnJumpServerRpc();
            Jump();
        }

        [ServerRpc]
        private void OnJumpServerRpc()
        {
            OnJumpClientRpc();
        }

        [ClientRpc]
        private void OnJumpClientRpc()
        {
            if (!IsOwner) Jump();
        }

        private void Jump()
        {
            _playerCharacterController.Jump();
            _playerWallRunController.WallJump();
        }

        /// <summary>
        ///     Called when the player sprints
        /// </summary>
        /// <param name="context"> The melee input context </param>
        public void OnMelee(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            if (IsOwner) OnMeleeServerRpc();
            Melee();
        }

        [ServerRpc]
        private void OnMeleeServerRpc()
        {
            OnMeleeClientRpc();
        }

        [ClientRpc]
        private void OnMeleeClientRpc()
        {
            if (!IsOwner) Melee();
        }

        private void Melee()
        {
            meleeKick.Attack();
        }
    }
}