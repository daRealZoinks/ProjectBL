using UnityEngine;
using LocalPlayer;

public class ArtificialIntelligence : MonoBehaviour
{
    [SerializeField] private PlayerCameraController playerCameraController;
    [SerializeField] private MeleeAttack meleeAttack;

    public GameObject Ball { get; set; }

    private PlayerCharacterController _playerCharacterController;

    private void Awake()
    {
        _playerCharacterController = GetComponent<PlayerCharacterController>();
    }

    private void Update()
    {
        if (Ball == null)
        {
            return;
        }

        var ballDirection = Ball.transform.position - transform.position;
        var ballDistance = ballDirection.magnitude;

        _playerCharacterController.Direction = ballDistance > 1.5f ? ballDirection.normalized : Vector3.zero;

        playerCameraController.Look = ballDirection.normalized;

        if (ballDistance < 1.5f)
        {
            meleeAttack.Attack();
        }
    }
}
