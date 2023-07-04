using UnityEngine;

public class BotTest : MonoBehaviour
{
    [SerializeField] private ArtificialIntelligence artificialIntelligence;

    private Transform _target;

    // make a function that gets the point i clicked on
    private void Update()
    {
        var clickedPoint = ClickedPoint();

        if (!clickedPoint.HasValue) return;

        // create a new game object at the clicked point and assign it to the target of the AI
        var target = new GameObject("Target");

        _target = target.transform;

        _target.position = clickedPoint.Value;

        artificialIntelligence.Target = _target;

        // destroy the target after 5 seconds
        Destroy(target, 5);
    }

    private Vector3? ClickedPoint()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit)) return hit.point;
        }

        return null;
    }
}