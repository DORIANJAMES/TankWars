using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;

public class HostGameManager
{
   [SerializeField] private string gameSceneName = "BigMap";
   private const int MaxConnections = 20;
   private Allocation _allocation;
   private string _joinCode;
   private string lobbyId;
   
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
         Debug.Log(_joinCode);
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
         Lobby lobby = await Lobbies.Instance.CreateLobbyAsync("My Lobby", MaxConnections, lobbyOptions);
         lobbyId = lobby.Id;
         HostSingleton.Instance.StartCoroutine(HeartbeatLobby(15));
      }
      catch (LobbyServiceException e)
      {
         Console.WriteLine(e);
         return;
      }

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
}
