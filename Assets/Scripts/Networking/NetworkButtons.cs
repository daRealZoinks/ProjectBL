using Unity.Netcode;
using UnityEngine;

namespace Networking
{
    public class NetworkButtons : MonoBehaviour
    {
        [SerializeField] private CursorController cursorController;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));

            var networkManager = NetworkManager.Singleton;
            if (!networkManager.IsClient && !networkManager.IsServer)
            {
                if (GUILayout.Button("Host"))
                {
                    networkManager.StartHost();
                    cursorController.HideCursor = true;
                }

                if (GUILayout.Button("Client"))
                {
                    networkManager.StartClient();
                    cursorController.HideCursor = true;
                }

                if (GUILayout.Button("Server"))
                {
                    networkManager.StartServer();
                    cursorController.HideCursor = true;
                }
            }

            GUILayout.EndArea();
        }
    }
}