using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private float _attackRange = 3f;
    [SerializeField] private float _attackRadius = 0.75f;
    [SerializeField] private float _attackForce = 250f;

    public float AttackRange { get; }

    public void Attack()
    {
        var hits = Physics.OverlapCapsule(transform.position, transform.position + transform.forward * _attackRange,
            _attackRadius);

        foreach (var hit in hits)
            if (hit.TryGetComponent<Rigidbody>(out var rigidbody))
                rigidbody.AddForce(transform.forward * _attackForce, ForceMode.Impulse);
    }
}