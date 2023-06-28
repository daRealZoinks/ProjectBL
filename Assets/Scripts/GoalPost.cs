using UnityEngine;
using UnityEngine.Events;

public class GoalPost : MonoBehaviour
{
    public event UnityAction OnGoal;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            OnGoal?.Invoke();
        }
    }
}
