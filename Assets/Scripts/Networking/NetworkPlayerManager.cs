using Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace Networking
{
    public class NetworkPlayerManager : NetworkBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        [SerializeField] private Behaviour[] componentsToDisable;
        [SerializeField] private GameObject[] objectsToDestroy;

        public override void OnNetworkSpawn()
        {
            if (IsOwner) return;

            foreach (var component in componentsToDisable) component.enabled = false;

            foreach (var obj in objectsToDestroy) Destroy(obj);

            virtualCamera.Priority = 5;
        }
    }
}