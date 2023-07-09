using UnityEngine;
using UnityEngine.Events;

namespace LocalPlayer
{
    public class GoalPost : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Ball")) OnGoal?.Invoke();
        }

        /// <summary>
        ///     Invoked when the ball enters the goal.
        /// </summary>
        public event UnityAction OnGoal;
    }
}