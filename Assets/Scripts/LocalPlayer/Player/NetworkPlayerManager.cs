using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace LocalPlayer.Player
{
    public class NetworkPlayerManager : NetworkBehaviour
    {
        [SerializeField]
        private UnityEvent OnIfOwner;

        [SerializeField]
        private UnityEvent OnIfNotOwner;

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                OnIfOwner?.Invoke();
            }
            else
            {
                OnIfNotOwner?.Invoke();
            }
        }
    }
}