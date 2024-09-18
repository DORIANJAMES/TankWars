using System;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : IDisposable
{
    private JoinAllocation _allocation;
    private NetworkClient _networkClient;
    private MatchplayMatchmaker _matchmaker;
    private UserData _userData;
    
    [SerializeField] private const string MenuSceneName = "Menu";
    
    public async Task<bool> InitAsync()
    {
        // Authenticate Player
        await UnityServices.InitializeAsync();

        _networkClient = new NetworkClient(NetworkManager.Singleton);
        _matchmaker = new MatchplayMatchmaker();
        
        AuthState authState = await AuthenticationWrapper.DoAuth();

        if (authState == AuthState.Authenticated)
        {
            _userData = new UserData
            {
                userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
                userAuthId = AuthenticationService.Instance.PlayerId
            };
            return true;
        }

        return false;

    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }

    public void StartClient(string ip, int port)
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip,(ushort)port);
        ConnectClient();
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            _allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = new RelayServerData(_allocation, "dtls");
        transport.SetRelayServerData(relayServerData);
        
        ConnectClient();
    }
    
    public void ConnectClient()
    {
        string payLoad = JsonUtility.ToJson(_userData);
        byte[] payLoadBytes = Encoding.UTF8.GetBytes(payLoad);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payLoadBytes;
        NetworkManager.Singleton.StartClient();
    }

    public async void MatchMakeAsync(Action<MatchmakerPollingResult> onMatchMakeResponse)
    {
        if (_matchmaker.IsMatchmaking)
        {
            return;
        }

        MatchmakerPollingResult matchResult = await GetMatchAsync();
        onMatchMakeResponse?.Invoke(matchResult);
    }

    private async Task<MatchmakerPollingResult> GetMatchAsync()
    {
        MatchmakingResult matchmakingResult = await _matchmaker.Matchmake(_userData);

        if (matchmakingResult.result == MatchmakerPollingResult.Success)
        {
            StartClient(matchmakingResult.ip, matchmakingResult.port);
            
        }

        return matchmakingResult.result;
    }
    
    public async Task CancelMatchMaking()
    {
        await _matchmaker.CancelMatchmaking();
    }
    
    public void Disconnect()
    {
        _networkClient.Disconnect();
    }
    
    public void Dispose()
    {
        _networkClient?.Dispose();
    }


    
}
