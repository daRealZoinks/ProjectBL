using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
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

            var unityTransport = (UnityTransport)networkManager.NetworkConfig.NetworkTransport;

            if (!networkManager.IsClient && !networkManager.IsServer)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("IP Address");
                unityTransport.ConnectionData.Address = GUILayout.TextField(unityTransport.ConnectionData.Address);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Port");
                unityTransport.ConnectionData.Port = ushort.Parse(GUILayout.TextField(unityTransport.ConnectionData.Port.ToString()));
                GUILayout.EndHorizontal();

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