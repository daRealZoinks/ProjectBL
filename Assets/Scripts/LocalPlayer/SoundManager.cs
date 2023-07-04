using UnityEngine;

namespace LocalPlayer
{
    public class SoundManager : MonoBehaviour
    {
        private AudioSource _audioSource;
        private PlayerCharacterController _playerCharacterController;
        private PlayerWallRunController _playerWallRunController;

        private void Awake()
        {
            _playerCharacterController = GetComponent<PlayerCharacterController>();
            _playerWallRunController = GetComponent<PlayerWallRunController>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (_playerCharacterController.IsGrounded || _playerWallRunController.IsWallRunning)
            {
                var horizontalMovement = new Vector3(_playerCharacterController.Movement.x, 0,
                    _playerCharacterController.Movement.z);
                _audioSource.mute = !(horizontalMovement.magnitude > 0.1f);
            }
            else
            {
                _audioSource.mute = true;
            }
        }
    }
}