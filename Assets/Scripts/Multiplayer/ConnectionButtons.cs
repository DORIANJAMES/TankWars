using Unity.Netcode;
using UnityEngine;

namespace Multiplayer
{
    public class ConnectionButtons : MonoBehaviour
    {
        public void HostServer() {
            NetworkManager.Singleton.StartHost();
        }  
        public void StartClient () {
            NetworkManager.Singleton.StartClient();
        }
    }
}
