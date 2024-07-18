using System;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;

public class HostGameManager
{
   [SerializeField] private string gameSceneName = "BigMap";
   private const int MaxConnections = 20;
   private Allocation _allocation;
   private string joinCode;
   
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
         joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
         Debug.Log(joinCode);
      }
      catch (Exception e)
      {
         Debug.LogError(e);
         return;
      }

      UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
      RelayServerData relayServerData = new RelayServerData(_allocation,"udp");
      transport.SetRelayServerData(relayServerData);

      NetworkManager.Singleton.StartHost();

      NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
   }
}
