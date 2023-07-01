using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeamPod : MonoBehaviour
{
    [SerializeField] private string sceneName;

    private void Update()
    {
        var offset = (transform.right + transform.forward) * 2.5f + 3 * transform.up;

        var colliders = Physics.OverlapBox(transform.position + offset, 2.5f * Vector3.one, Quaternion.identity);

        List<Collider> playerColliders = new();
        foreach (var collider in colliders)
            if (collider.CompareTag("Player"))
                playerColliders.Add(collider);

        List<NetworkObject> networkObjects = new();

        foreach (var playerCollider in playerColliders)
            networkObjects.Add(playerCollider.GetComponent<NetworkObject>());

        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
            foreach (var networkObject in networkObjects)
                if (networkObject.IsOwnedByServer)
                    NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;

    private void OnValidate()
    {
        if (sceneAsset != null) sceneName = sceneAsset.name;
    }
#endif
}