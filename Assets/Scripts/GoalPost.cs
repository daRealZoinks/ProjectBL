using UnityEngine;
using UnityEngine.Events;

public class GoalPost : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball")) OnGoal?.Invoke();
    }

    public event UnityAction OnGoal;
}