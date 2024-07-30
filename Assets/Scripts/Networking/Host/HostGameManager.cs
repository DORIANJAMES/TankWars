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

public class HostGameManager : IDisposable
{
   [SerializeField] private string gameSceneName = "BigMap";
   private const int MaxConnections = 20;
   private Allocation _allocation;
   private string _joinCode;
   private string lobbyId;
   private NetworkServer _networkServer;
   
   public async Task StartHostAsync()
   {
      try
      {
         _allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
      }
      catch (Exception e)
      {
         Debug.LogError(e);
         return;
      }
      
      try
      {
         _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
         PlayerPrefs.SetString("JoinCode",_joinCode);
      }
      catch (Exception e)
      {
         Debug.LogError(e);
         return;
      }

      UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
      RelayServerData relayServerData = new RelayServerData(_allocation,"dtls");
      transport.SetRelayServerData(relayServerData);

      // Lobby is creating
      try
      {
         CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
         lobbyOptions.IsPrivate = false;
         lobbyOptions.Data = new Dictionary<string, DataObject>()
         {
            {
               "JoinCode",new DataObject(
                     visibility:DataObject.VisibilityOptions.Member,
                     value:_joinCode
                  )
            }
         };
         string playerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Unknown");
         Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(playerName+"'s Lobby", MaxConnections, lobbyOptions);
         lobbyId = lobby.Id;
         HostSingleton.Instance.StartCoroutine(HeartbeatLobby(15));
      }
      catch (LobbyServiceException e)
      {
         Console.WriteLine(e);
         return;
      }

      _networkServer = new NetworkServer(NetworkManager.Singleton);
      
      UserData userData = new UserData
      {
         userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
         userAuthId = AuthenticationService.Instance.PlayerId
      };
        
      string payLoad = JsonUtility.ToJson(userData);
      byte[] payLoadBytes = Encoding.UTF8.GetBytes(payLoad);

      NetworkManager.Singleton.NetworkConfig.ConnectionData = payLoadBytes;

      NetworkManager.Singleton.StartHost();

      NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
   }

   private IEnumerator HeartbeatLobby(float waitTimeSeconds)
   {
      WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
      while (true)
      {
         Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
         yield return delay;
      }
   }

   public async void Dispose()
   {
      HostSingleton.Instance.StopCoroutine(nameof(HeartbeatLobby));
      if (string.IsNullOrEmpty(lobbyId))
      {
         try
         {
            await Lobbies.Instance.DeleteLobbyAsync(lobbyId);
         }
         catch (LobbyServiceException e)
         {
            Debug.Log(e);
            return;
         }

         lobbyId = string.Empty;
      }
      _networkServer?.Dispose();
   }
}
