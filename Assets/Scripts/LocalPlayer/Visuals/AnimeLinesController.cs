using LocalPlayer.Player;
using UnityEngine;
using UnityEngine.VFX;

namespace LocalPlayer.Visuals
{
    public class AnimeLinesController : MonoBehaviour
    {
        [Tooltip("Reference to a generic character movement script")]
        [SerializeField]
        private CharacterMovement characterMovement;

        [Tooltip("Reference to the anime lines visual effect")]
        [SerializeField]
        private VisualEffect animeLines;

        [Tooltip("The maximum speed at which the anime lines will be visible")]
        [SerializeField]
        private float maxSpeed = 30f;

        private void Start()
        {
            animeLines.enabled = true;
            animeLines.Stop();
        }

        private void Update()
        {
            if (characterMovement.Velocity.magnitude >= maxSpeed)
                animeLines.Play();
            else
                animeLines.Stop();
        }
    }
}