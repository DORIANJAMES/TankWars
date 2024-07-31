using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;


public class NetworkServer : IDisposable
{
    private NetworkManager _networkManager;
    private Dictionary<ulong, string> _clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> _authIdToUserData = new Dictionary<string, UserData>();
    public NetworkServer(NetworkManager networkManager)
    {
        this._networkManager = networkManager;
        _networkManager.ConnectionApprovalCallback += ApprovalCheck;
        _networkManager.OnServerStarted += OnNetworkReady;
    }

   

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        //Converting Byte to String.
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        // Converting String to Json
        UserData userData = JsonUtility.FromJson<UserData>(payload);
        
        _clientIdToAuth[request.ClientNetworkId] = userData.userAuthId;
        _authIdToUserData[userData.userAuthId] = userData;
        
        response.Approved = true;
        response.CreatePlayerObject = true;
    }
    
    private void OnNetworkReady()
    {
        _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    public UserData GetUserDataByClientId(ulong clientId)
    {
        if (_clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            if (_authIdToUserData.TryGetValue(authId, out UserData data))
            {
                return data;
            }
            return null;
        }
        return null;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (_clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            _clientIdToAuth.Remove(clientId);
            _authIdToUserData.Remove(authId);
        }
    }

    public void Dispose()
    {
        if (_networkManager != null)
        {
            _networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            _networkManager.OnServerStarted -= OnNetworkReady;
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
            if (_networkManager.IsListening)
            {
                _networkManager.Shutdown();
            }
        }
    }
}
