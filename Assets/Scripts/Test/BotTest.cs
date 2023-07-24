using LocalPlayer.Player;
using UnityEngine;

namespace Test
{
    public class BotTest : MonoBehaviour
    {
        [Tooltip("The artificial intelligence to use for moving towards the target.")]
        [SerializeField]
        private ArtificialIntelligence artificialIntelligence;

        private Transform _target;

        private void Update()
        {
            var clickedPoint = ClickedPoint();

            if (clickedPoint == null) return;

            var target = new GameObject("Target");

            _target = target.transform;

            _target.position = clickedPoint.Value;

            artificialIntelligence.MoveTarget = _target;
            artificialIntelligence.LookTarget = _target;

            Destroy(target, 5);
        }

        private void OnDrawGizmos()
        {
            if (_target == null) return;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_target.position, 1f);
        }

        private static Vector3? ClickedPoint()
        {
            if (!Input.GetMouseButtonDown(0)) return null;
            if (!Camera.main) return null;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit)) return hit.point;

            return null;
        }
    }
}