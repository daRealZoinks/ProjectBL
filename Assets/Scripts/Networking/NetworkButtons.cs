using Unity.Netcode;
using UnityEngine;

namespace Networking
{
    public class NetworkButtons : MonoBehaviour
    {
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));

            var networkManager = NetworkManager.Singleton;
            if (!networkManager.IsClient && !networkManager.IsServer)
            {
                if (GUILayout.Button("Host")) networkManager.StartHost();
                if (GUILayout.Button("Client")) networkManager.StartClient();
                if (GUILayout.Button("Server")) networkManager.StartServer();
            }

            GUILayout.EndArea();
        }
    }
}