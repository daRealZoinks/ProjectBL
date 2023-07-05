using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyPredictor : MonoBehaviour
{
    public float predictionTime = 1f; // Time in seconds to predict ahead
    public int predictionSteps = 20; // Number of steps to divide the prediction time
    private Rigidbody rb;
    private List<Vector3> predictedPath = new List<Vector3>();

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        predictedPath.Clear();

        for (int i = 0; i < predictionSteps; i++)
        {
            float predictionTimeStep = predictionTime / predictionSteps * i;

            if (rb.SweepTest(rb.velocity.normalized, out var hit, rb.velocity.magnitude * predictionTimeStep))
            {
                predictedPath.Add(hit.point);
                break;
            }
            else
            {
                Vector3 velocityWithGravity = rb.velocity + Physics.gravity * predictionTimeStep;
                Vector3 predictedPosition = transform.position + velocityWithGravity * predictionTimeStep;
                predictedPath.Add(predictedPosition);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < predictedPath.Count - 1; i++)
        {
            Gizmos.DrawLine(predictedPath[i], predictedPath[i + 1]);
        }
    }
}
