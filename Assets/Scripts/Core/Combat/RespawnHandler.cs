using System.Collections;
using System.Collections.Generic;
using Core.Combat;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);

        foreach (var player in players)
        {
            HandlePlayerSpawned(player);
        }
        
        TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawn += HandlePlayerDespawned;
    }

    

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
        {
            return;
        }
        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawn -= HandlePlayerDespawned;
    }
    
    private void HandlePlayerSpawned(TankPlayer player)
    {
        player.Health.OnDeath += (health) => HandlePlayerDie(player);
    }
    private void HandlePlayerDespawned(TankPlayer player)
    {
        player.Health.OnDeath -= (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDie(TankPlayer player)
    {
        Destroy(player.gameObject);
        StartCoroutine(RespawnPlayer(player.OwnerClientId));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientId)
    {
        yield return null;
        NetworkObject playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);
        
        playerInstance.SpawnAsPlayerObject(ownerClientId);
    }
}
