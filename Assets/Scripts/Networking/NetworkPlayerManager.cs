using Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace Networking
{
    public class NetworkPlayerManager : NetworkBehaviour
    {
        [Tooltip("The virtual camera that will follow the player")] [SerializeField]
        private CinemachineVirtualCamera virtualCamera;

        [Space]
        [Header("Non-Owner Settings")]
        [Tooltip("The components to disable on non-owned players")]
        [SerializeField]
        private Behaviour[] componentsToDisable;

        [Tooltip("The objects to destroy on non-owned players")] [SerializeField]
        private GameObject[] objectsToDestroy;

        public override void OnNetworkSpawn()
        {
            if (IsOwner) return;

            foreach (var component in componentsToDisable) component.enabled = false;
            foreach (var obj in objectsToDestroy) Destroy(obj);
            virtualCamera.Priority = 5;
        }
    }
}