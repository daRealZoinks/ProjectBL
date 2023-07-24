using LocalPlayer.Player;
using UnityEngine;
using UnityEngine.VFX;

namespace LocalPlayer.Visuals
{
    public class LandingPuff : MonoBehaviour
    {
        [Tooltip("The threshold for the fall speed to trigger the landing puff.")]
        [SerializeField]
        private float threshold = 10f;

        [Tooltip("The character controller to listen to for landing events.")]
        [SerializeField]
        private CharacterMovement characterMovement;

        private VisualEffect _visualEffect;

        private void Start()
        {
            _visualEffect = GetComponent<VisualEffect>();

            characterMovement.OnLanded += CharacterMovement_OnLanded;
        }

        private void CharacterMovement_OnLanded(float fallSpeed)
        {
            if (fallSpeed > threshold)
            {
                _visualEffect.Play();
            }
        }
    }
}
