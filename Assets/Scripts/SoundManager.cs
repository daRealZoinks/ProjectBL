using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private PlayerCharacterController _playerCharacterController;
    private AudioSource _audioSource;

    private void Awake()
    {
        _playerCharacterController = GetComponent<PlayerCharacterController>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_playerCharacterController.Grounded)
        {
            _audioSource.mute = !(_playerCharacterController.Direction.magnitude > 0);
        }
        else
        {
            _audioSource.mute = true;
        }
    }
}
