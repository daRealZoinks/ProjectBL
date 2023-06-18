using UnityEngine;
using UnityEngine.VFX;

public class AnimeLinesController : MonoBehaviour
{
    [SerializeField] private PlayerCharacterController playerCharacterController;
    [SerializeField] private VisualEffect animeLines;
    [SerializeField] private float superSpeed = 50f;

    private void Start()
    {
        animeLines.enabled = true;
        animeLines.Stop();
    }

    private void Update()
    {
        if (playerCharacterController.Movement.magnitude > superSpeed)
        {
            animeLines.Play();
        }
        else
        {
            animeLines.Stop();
        }
    }
}
