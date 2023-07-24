using Unity.Netcode;
using UnityEngine;

namespace LocalPlayer.Player
{
    public class NewMonoBehaviour : NetworkBehaviour, INetworkPrediction
    {
        [Tooltip("The network smoothing mode to use for this object.")]
        [SerializeField]
        private NetworkSmoothingMode networkSmoothingMode = NetworkSmoothingMode.Exponential;

        [Tooltip("The speed at which to smooth the object's position and rotation.")]
        [SerializeField]
        private float networkSmoothingSpeed;

        private bool _networkSmoothingComplete;

        public StatePayload PredictionData_Client { get; }

        public StatePayload PredictionData_Server { get; }

        public bool ForcePositionUpdate(float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public bool HasPredictionData_Client()
        {
            throw new System.NotImplementedException();
        }

        public bool HasPredictionData_Server()
        {
            throw new System.NotImplementedException();
        }

        public void ResetPredictionData_Client()
        {
            throw new System.NotImplementedException();
        }

        public void ResetPredictionData_Server()
        {
            throw new System.NotImplementedException();
        }

        public void SendClientAdjustment()
        {
            throw new System.NotImplementedException();
        }

        public void SmoothCorrection(Vector3 oldPosition, Quaternion oldRotation, Vector3 newPosition, Quaternion newRotation)
        {
            if (IsClient || IsHost) return;

            if (IsOwner) return;

            _networkSmoothingComplete = false;

            switch (networkSmoothingMode)
            {
                case NetworkSmoothingMode.Disabled:
                    transform.SetPositionAndRotation(newPosition, newRotation);
                    _networkSmoothingComplete = true;
                    break;
                case NetworkSmoothingMode.Linear:
                    var linearValue = 1 - Time.deltaTime * networkSmoothingSpeed;
                    transform.position = Vector3.Lerp(oldPosition, newPosition, linearValue);
                    transform.rotation = Quaternion.Lerp(oldRotation, newRotation, linearValue);
                    _networkSmoothingComplete = true;
                    break;
                case NetworkSmoothingMode.Exponential:
                    var exponentialValue = Mathf.Pow(0.01f, Time.deltaTime * networkSmoothingSpeed);
                    transform.position = Vector3.Lerp(newPosition, oldPosition, exponentialValue);
                    transform.rotation = Quaternion.Lerp(newRotation, oldRotation, exponentialValue);
                    _networkSmoothingComplete = true;
                    break;
                default:
                    break;
            }
        }
    }
}