using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Image healPowerBar;


    [Header("Settings")] 
    [SerializeField] private int maxHealPower = 30;
    [SerializeField] private float healCooldown = 60f;
    [SerializeField] private float healTickRate = 1f;
    [SerializeField] private int coinsPerTick = 10;
    [SerializeField] private int healthPerTick = 10;

    [SerializeField] private List<TankPlayer> playersInZone = new List<TankPlayer>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) { return; }
        if (!other.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }
        playersInZone.Add(player);
        Debug.Log($"{player.playerName.Value} has Entered...!");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsServer) { return; }
        if (!other.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }
        playersInZone.Remove(player);
        Debug.Log($"{player.playerName.Value} has Left...!");
    }
}
