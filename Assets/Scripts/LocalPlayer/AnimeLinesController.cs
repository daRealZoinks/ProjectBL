using UnityEngine;
using UnityEngine.VFX;

namespace LocalPlayer
{
    public class AnimeLinesController : MonoBehaviour
    {
        [Tooltip("Reference to the player character controller")]
        [SerializeField] private PlayerCharacterController playerCharacterController;

        [Tooltip("Reference to the anime lines visual effect")]
        [SerializeField] private VisualEffect animeLines;

        [Tooltip("The maximum speed at which the anime lines will be visible")]
        [SerializeField] private float maxSpeed = 30f;

        private void Start()
        {
            animeLines.enabled = true;
            animeLines.Stop();
        }

        private void Update()
        {
            if (playerCharacterController.Velocity.magnitude >= maxSpeed)
                animeLines.Play();
            else
                animeLines.Stop();
        }
    }
}