using LocalPlayer;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Networking
{
    public class NetworkButtons : MonoBehaviour
    {
        [SerializeField] private CursorController cursorController;

        private UnityTransport _unityTransport;

        private void Start()
        {
            _unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));

            var networkManager = NetworkManager.Singleton;

            if (!networkManager.IsClient && !networkManager.IsServer)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("IP Address");
                _unityTransport.ConnectionData.Address = GUILayout.TextField(_unityTransport.ConnectionData.Address);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Port");
                _unityTransport.ConnectionData.Port =
                    ushort.Parse(GUILayout.TextField(_unityTransport.ConnectionData.Port.ToString()));
                GUILayout.EndHorizontal();

                if (GUILayout.Button("Host")) networkManager.StartHost();
                if (GUILayout.Button("Client")) networkManager.StartClient();
                if (GUILayout.Button("Server")) networkManager.StartServer();
            }

            cursorController.HideCursor = networkManager.IsClient;

            GUILayout.EndArea();
        }
    }
}