using LocalPlayer.Player;
using UnityEngine;

namespace LocalPlayer.Sound
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(PlayerWallRunController))]
    public class SoundManager : MonoBehaviour
    {
        private AudioSource _audioSource;
        private CharacterMovement _characterMovement;
        private PlayerWallRunController _playerWallRunController;

        private void Awake()
        {
            _characterMovement = GetComponent<CharacterMovement>();
            _playerWallRunController = GetComponent<PlayerWallRunController>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (_characterMovement.IsGrounded || _playerWallRunController.IsWallRunning)
            {
                Vector3 horizontalMovement = new(_characterMovement.Velocity.x, 0,
                    _characterMovement.Velocity.z);

                _audioSource.mute = !(horizontalMovement.magnitude > 0.1f);
            }
            else
            {
                _audioSource.mute = true;
            }
        }
    }
}