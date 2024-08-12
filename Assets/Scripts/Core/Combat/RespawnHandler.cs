using System.Collections;
using System.Collections.Generic;
using Core.Combat;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private TankPlayer playerPrefab;
    [SerializeField] private float keptCoinPercentage;
    [SerializeField] private int keptCoins;
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
        keptCoins = (int) (player.Wallet.totalCoins.Value * (keptCoinPercentage / 100));
        Destroy(player.gameObject);
        StartCoroutine(RespawnPlayer(player.OwnerClientId, keptCoins));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientId, int newCoins)
    {
        yield return null;
        TankPlayer playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);
        
        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientId);
        playerInstance.Wallet.totalCoins.Value += newCoins;
    }
}
