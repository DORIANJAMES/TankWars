using System;
using System.Collections;
using System.Collections.Generic;
using Core.Combat;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CoinWallet : NetworkBehaviour
{
    [Header("Referenes")]
    [SerializeField] private Health health;
    [SerializeField] private BountyCoin bountyCoinPrefab;


    [Header("Settings")] 
    [SerializeField] private float coinSpread =3f;
    [SerializeField] private float bountyPercentage = 50f;
    [SerializeField] private int bountyCoinCount = 10;
    [SerializeField] private int minBountyCoinValue = 5;
    [SerializeField] private LayerMask layerMask;
    private Collider2D[] _coinBuffer = new Collider2D[1];
    private float _coinRadius;
    
    public NetworkVariable<int> totalCoins = new NetworkVariable<int>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<Coin>(out Coin coin)) { return; }

        int coinValue = coin.Collect();
        
        
        if (!IsServer) { return; }

        totalCoins.Value += coinValue;
    }

    public void SpendCoins(int costFire)
    {
        totalCoins.Value -= costFire;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }
        _coinRadius = bountyCoinPrefab.GetComponent<CircleCollider2D>().radius;
        health.OnDeath += HandleDie;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }
        health.OnDeath -= HandleDie;
    }

    private void HandleDie(Health health)
    {
        int bountyValue = (int) (totalCoins.Value * (bountyPercentage / 100f));
        int bountyCoinValue = bountyValue / bountyCoinCount;

        if (bountyCoinValue < minBountyCoinValue) { return; }

        for (int i = 0; i < bountyCoinCount; i++)
        {
            BountyCoin bountyCoinInstance = Instantiate(bountyCoinPrefab,GetSpawnPoint(),Quaternion.identity);
            bountyCoinInstance.SetValue(bountyCoinValue);
            bountyCoinInstance.NetworkObject.Spawn();
        }
    }
    
    private Vector2 GetSpawnPoint()
    {
        while (true)
        {
            Vector2 spawnPoint = (Vector2)transform.position + Random.insideUnitCircle * coinSpread;
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, _coinRadius, _coinBuffer, layerMask);
            if (numColliders == 0) { return spawnPoint; }
        } 
    }
}