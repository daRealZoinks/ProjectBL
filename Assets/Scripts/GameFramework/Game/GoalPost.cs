using UnityEngine;
using UnityEngine.Events;

namespace GameFramework.Game
{
    public class GoalPost : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Ball")) OnGoal?.Invoke();
            Debug.Log("Goal!");
        }

        /// <summary>
        ///     Invoked when the ball enters the goal.
        /// </summary>
        public event UnityAction OnGoal;
    }
}