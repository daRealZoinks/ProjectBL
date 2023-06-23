using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class NetworkPlayerManager : NetworkBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SerializeField] private Behaviour[] componentsToDisable;
    [SerializeField] private GameObject[] objectsToDestroy;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            foreach (var component in componentsToDisable)
            {
                Destroy(component);
            }

            foreach (var obj in objectsToDestroy)
            {
                Destroy(obj);
            }

            virtualCamera.Priority = 5;
        }
    }
}
