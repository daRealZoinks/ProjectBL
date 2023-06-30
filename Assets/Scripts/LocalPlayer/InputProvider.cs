using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LocalPlayer
{
    public class InputProvider : MonoBehaviour
    {
        [SerializeField] private PlayerCameraController playerCameraController;
        [SerializeField] private MeleeAttack meleeAttack;
        [SerializeField] private float jumpBufferringTime = 0.5f;

        private PlayerCharacterController _playerCharacterController;

        private void Awake()
        {
            _playerCharacterController = GetComponent<PlayerCharacterController>();
        }

        public void OnMove(InputValue value)
        {
            _playerCharacterController.Direction = value.Get<Vector2>();
        }

        public void OnLook(InputValue value)
        {
            playerCameraController.Look = value.Get<Vector2>();
        }

        public void OnFire(InputValue value)
        {
            // shooting logic
        }

        public async void OnJump(InputValue value)
        {
            if (value.isPressed)
            {
                _playerCharacterController.Jumping = true;
                await Task.Delay(TimeSpan.FromSeconds(jumpBufferringTime));
                _playerCharacterController.Jumping = false;
            }
        }

        public void OnMelee(InputValue value)
        {
            if (value.isPressed)
            {
                meleeAttack.Attack();
            }
        }
    }
}