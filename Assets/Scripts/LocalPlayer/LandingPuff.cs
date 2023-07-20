using LocalPlayer;
using UnityEngine;
using UnityEngine.VFX;

public class LandingPuff : MonoBehaviour
{
    // float for threshold
    [Tooltip("The threshold for the fall speed to trigger the landing puff.")]
    [SerializeField]
    private float threshold = 10f;

    [Tooltip("The character controller to listen to for landing events.")]
    [SerializeField]
    private PlayerCharacterController playerCharacterController;

    private VisualEffect _visualEffect;

    private void Start()
    {
        _visualEffect = GetComponent<VisualEffect>();

        playerCharacterController.OnLand += OnLanded;
    }

    private void OnLanded(float fallSpeed)
    {
        if (fallSpeed > threshold)
        {
            _visualEffect.Play();
        }
    }
}
