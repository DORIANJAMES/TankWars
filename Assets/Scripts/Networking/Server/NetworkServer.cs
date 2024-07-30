using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class NetworkServer
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

        Debug.Log(userData.userName);

        _clientIdToAuth[request.ClientNetworkId] = userData.userAuthId;
        _authIdToUserData[userData.userAuthId] = userData;
        
        response.Approved = true;
        response.CreatePlayerObject = true;
    }
    
    private void OnNetworkReady()
    {
        _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (_clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            Debug.Log($"Client ID = {clientId.ToString()} disconnected");
            Debug.Log($"Auth ID = {authId} disconnected");
            _clientIdToAuth.Remove(clientId);
            _authIdToUserData.Remove(authId);
        }
    }
}
