using Networking;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class TeamPodManager : MonoBehaviour
{
    [Tooltip("The scene to load when all players are in the pod.")]
    [SerializeField]
    private string sceneName;

    [Tooltip("The pods to check for players.")]
    [SerializeField]
    private TeamPod[] pods;

    void Update()
    {
        var networkManager = NetworkManager.Singleton;

        if (!networkManager.IsServer || !networkManager.IsHost) return;

        var numberOfPodsReady = 0;

        foreach (var pod in pods) if (pod.IsReady) numberOfPodsReady++;

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
