using UnityEngine;
using UnityEngine.VFX;

namespace LocalPlayer
{
    public class AnimeLinesController : MonoBehaviour
    {
        [SerializeField] private PlayerCharacterController playerCharacterController;
        [SerializeField] private VisualEffect animeLines;

        private void Start()
        {
            animeLines.enabled = true;
            animeLines.Stop();
        }

        private void Update()
        {
            if (playerCharacterController.Movement.magnitude > playerCharacterController.MaxSpeed * 2)
                animeLines.Play();
            else
                animeLines.Stop();
        }
    }
}