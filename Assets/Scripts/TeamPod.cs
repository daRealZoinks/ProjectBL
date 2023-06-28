using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class TeamPod : MonoBehaviour
{
    [SerializeField] private string sceneName;

#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;

    private void OnValidate()
    {
        if (sceneAsset != null)
        {
            sceneName = sceneAsset.name;
        }
    }
#endif

    private void Update()
    {
        var offset = (Vector3.right + Vector3.forward) * 2.5f + 3 * Vector3.up;

        var colliders = Physics.OverlapBox(transform.position + offset, 2.5f * Vector3.one, Quaternion.identity);

        List<Collider> playerColliders = new();
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                playerColliders.Add(collider);
            }
        }

        List<NetworkObject> networkObjects = new();

        foreach (var playerCollider in playerColliders)
        {
            networkObjects.Add(playerCollider.GetComponent<NetworkObject>());
        }

        foreach (var networkObject in networkObjects)
        {
            if (networkObject.IsOwnedByServer)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            }
        }
    }
}
