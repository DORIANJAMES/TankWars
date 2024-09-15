using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ServerGameManager : IDisposable
{
    private string _serverIp;
    private int _serverPort;
    private int _queryPort;
    private NetworkServer _networkServer;
    private MultiplayAllocationService _multiplayAllocationService;
    
    private const string GameSceneName = "Game";
    public ServerGameManager(string serverIp, int serverPort, int serverQPort, NetworkManager manager)
    {
        this._serverIp = serverIp;
        this._serverPort = serverPort;
        this._queryPort = serverQPort;
        _networkServer = new NetworkServer(manager);
        _multiplayAllocationService = new MultiplayAllocationService();
    }
    public async Task StartGameServerAsync()
    {
        await _multiplayAllocationService.BeginServerCheck();
        if (_networkServer.OpenConnection(_serverIp, _serverPort))
        {
            Debug.LogWarning("Networkserver cannot be started");
            return;
        }

        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }
    public void Dispose()
    {
        _multiplayAllocationService?.Dispose();
        _networkServer?.Dispose();
    }

    
}
