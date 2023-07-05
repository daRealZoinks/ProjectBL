using System.Collections.Generic;
using UnityEngine;

namespace LocalPlayer
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyPredictor : MonoBehaviour
    {
        public float predictionTime = 1f; // Time in seconds to predict ahead
        public int predictionSteps = 20; // Number of steps to divide the prediction time
        private Rigidbody _rigidbody;
        private readonly List<Vector3> _predictedPath = new();

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            _predictedPath.Clear();

            for (var i = 0; i < predictionSteps; i++)
            {
                var predictionTimeStep = predictionTime / predictionSteps * i;

                var rigidbodyVelocity = _rigidbody.velocity;
                
                var sweepTest = _rigidbody.SweepTest(rigidbodyVelocity.normalized, out var hit,
                    rigidbodyVelocity.magnitude * predictionTimeStep);

                if (sweepTest)
                {
                    _predictedPath.Add(hit.point);
                    break;
                }

                var velocityWithGravity = _rigidbody.velocity + Physics.gravity * predictionTimeStep;
                var predictedPosition = transform.position + velocityWithGravity * predictionTimeStep;
                _predictedPath.Add(predictedPosition);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            for (var i = 0; i < _predictedPath.Count - 1; i++)
            {
                Gizmos.DrawLine(_predictedPath[i], _predictedPath[i + 1]);
            }
        }
    }
}
