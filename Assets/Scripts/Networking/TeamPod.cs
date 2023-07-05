using System.Linq;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class TeamPod : MonoBehaviour
    {
        [Tooltip("The scene to load when all players are in the pod.")] [SerializeField]
        private string sceneName;

        private void Update()
        {
            var podTransform = transform;
            var offset = (podTransform.right + podTransform.forward) * 2.5f + 3 * podTransform.up;

            var colliders = new Collider[10];
            Physics.OverlapBoxNonAlloc(podTransform.position + offset, 2.5f * Vector3.one, colliders,
                Quaternion.identity);

            var playerColliders = colliders.Where(playerCollider => playerCollider.CompareTag("Player")).ToList();

            var networkObjects = playerColliders.Select(playerCollider => playerCollider.GetComponent<NetworkObject>())
                .ToList();

            if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost) return;
            foreach (var unused in networkObjects.Where(networkObject => networkObject.IsOwnedByServer))
                NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

#if UNITY_EDITOR
        [Tooltip("The scene to load when all players are in the pod.")] [SerializeField]
        private SceneAsset sceneAsset;

        private void OnValidate()
        {
            if (sceneAsset != null) sceneName = sceneAsset.name;
        }
#endif
    }
}