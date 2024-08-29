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
   public NetworkServer networkServer { get; private set; }
   
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

      networkServer = new NetworkServer(NetworkManager.Singleton);
      
      UserData userData = new UserData
      {
         userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
         userAuthId = AuthenticationService.Instance.PlayerId
      };
        
      string payLoad = JsonUtility.ToJson(userData);
      byte[] payLoadBytes = Encoding.UTF8.GetBytes(payLoad);

      NetworkManager.Singleton.NetworkConfig.ConnectionData = payLoadBytes;

      NetworkManager.Singleton.StartHost();
      networkServer.OnClientLeft += HandleClientLeft; 

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

   public void Dispose()
   {
      ShutDown();
   }

   public async void ShutDown()
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
      networkServer.OnClientLeft -= HandleClientLeft; 
      networkServer?.Dispose();
   }

   private async void HandleClientLeft(string authId)
   {
      try
      {
         await LobbyService.Instance.RemovePlayerAsync(lobbyId, authId);
      }
      catch (LobbyServiceException e)
      {
         Debug.Log(e);
      }
   }
}
