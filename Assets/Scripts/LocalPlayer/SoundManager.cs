using UnityEngine;

namespace LocalPlayer
{
    public class SoundManager : MonoBehaviour
    {
        private AudioSource _audioSource;
        private PlayerCharacterController _playerCharacterController;

        private void Awake()
        {
            _playerCharacterController = GetComponent<PlayerCharacterController>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (_playerCharacterController.Grounded)
            {
                var horizontalMovement = new Vector3(_playerCharacterController.Movement.x, 0, _playerCharacterController.Movement.z);
                _audioSource.mute = !(horizontalMovement.magnitude > 0.1f);
            }
            else
                _audioSource.mute = true;
        }
    }
}