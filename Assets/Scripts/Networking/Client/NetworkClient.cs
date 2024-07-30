using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager _networkManager;
    private const string MenuSceneName = "Menu";

    public NetworkClient(NetworkManager networkManager)
    {
        this._networkManager = networkManager;
        _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }
    
  

    private void OnClientDisconnect(ulong clientId)
    {
        // if we are a server to getting rid of shoting down the session every disconnection of other players.
        if (clientId != 0 && clientId != _networkManager.LocalClientId) { return; }
        
        // Incase we are in the wrong scene, we should go back to the menu scene.
        if (SceneManager.GetActiveScene().name != MenuSceneName)
        {
            SceneManager.LoadScene(MenuSceneName);
        }

        // Incase we remain as connected. Shooting down dhe session of related player.
        if (_networkManager.IsConnectedClient)
        {
            _networkManager.Shutdown();
        }
    }

    public void Dispose()
    {
        if (_networkManager != null)
        {
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }
}
