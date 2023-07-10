using System.Linq;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class TeamPodManager : MonoBehaviour
    {
        [Tooltip("The scene to load when all players are in the pod.")]
        [SerializeField]
        private string sceneName;

        [Tooltip("The pods to check for players.")]
        [SerializeField]
        private TeamPod[] pods;

        private void Update()
        {
            var networkManager = NetworkManager.Singleton;

            if (!networkManager.IsServer) return;

            var numberOfPodsReady = pods.Count(pod => pod.IsReady);

            if (networkManager.ConnectedClientsList.Count > 0 &&
                numberOfPodsReady == networkManager.ConnectedClientsList.Count)
                networkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

#if UNITY_EDITOR
        [Tooltip("The scene to load when all players are in the pod.")]
        [SerializeField]
        private SceneAsset sceneAsset;

        private void OnValidate()
        {
            if (sceneAsset != null) sceneName = sceneAsset.name;
        }
#endif
    }
}