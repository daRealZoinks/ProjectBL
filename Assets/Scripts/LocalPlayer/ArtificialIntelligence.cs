using UnityEngine;
using LocalPlayer;

public class ArtificialIntelligence : MonoBehaviour
{
    [SerializeField] private PlayerCameraController playerCameraController;
    [SerializeField] private MeleeAttack meleeAttack;
    [SerializeField] private Transform target;

    public Transform Target
    {
        get => target;
        set => target = value;
    }

    private PlayerCharacterController _playerCharacterController;

    private void Awake()
    {
        _playerCharacterController = GetComponent<PlayerCharacterController>();
    }

    private void Update()
    {
        if (Target == null)
        {
            _playerCharacterController.Direction = Vector2.zero;
            playerCameraController.Look = Vector2.zero;

            return;
        }

        _playerCharacterController.Direction = MoveTowards(Target.position);

        var distanceFromCharacterToTarget = (_playerCharacterController.transform.position - Target.position).magnitude;

        if (distanceFromCharacterToTarget < meleeAttack.AttackRange)
        {
            meleeAttack.Attack();
        }

        playerCameraController.Look = LookAt(Target.position);
    }

    private Vector2 MoveTowards(Vector3 target)
    {
        var directionToTarget = target - transform.position;
        directionToTarget.y = 0f; // Ignore the vertical component

        var localDirection = Quaternion.Inverse(transform.rotation) * directionToTarget;

        Vector2 directionToTake = new(localDirection.x, localDirection.z);

        return directionToTake;
    }

    private Vector2 LookAt(Vector3 target)
    {
        var directionToTarget = target - transform.position;
        var targetRotation = Quaternion.LookRotation(directionToTarget);
        var characterRotation = Quaternion.LookRotation(transform.forward);

        var characterRotationDifference = targetRotation * Quaternion.Inverse(characterRotation);

        var characterEulerRotation = characterRotationDifference.eulerAngles;
        characterEulerRotation.x = Mathf.Repeat(characterEulerRotation.x + 180f, 360f) - 180f;
        characterEulerRotation.y = Mathf.Repeat(characterEulerRotation.y + 180f, 360f) - 180f;

        var desiredRotation = new Vector2(characterEulerRotation.y, 0f); // TODO: make the bot look up and down

        return desiredRotation;
    }

    private void OnDrawGizmos()
    {
        if (Target == null) return;

        // draw a line from the character to the target
        Gizmos.color = Color.red;
        Gizmos.DrawLine(playerCameraController.transform.position, Target.position);

        // draw a line for the direction the character is moving
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, new Vector3(_playerCharacterController.Direction.x, 0f, _playerCharacterController.Direction.y) + transform.position);

        // draw a line for the direction the character is facing
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCameraController.transform.position, playerCameraController.transform.forward + playerCameraController.transform.position);
    }
}
