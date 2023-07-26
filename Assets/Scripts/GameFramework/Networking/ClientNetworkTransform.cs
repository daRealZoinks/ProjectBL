using Unity.Netcode.Components;

namespace GameFramework.Networking
{
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}