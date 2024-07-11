using System.Collections;
using System.Collections.Generic;
using Core.Combat;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [Header("References")] 
    [SerializeField] private Health health;
    [SerializeField] private Image _healthBarImage;


    public override void OnNetworkSpawn()
    {
        if (!IsClient)
            return;
        health.currentHealth.OnValueChanged += HandleHealthChange;
        HandleHealthChange(0, health.currentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient)
            return;
        health.currentHealth.OnValueChanged -= HandleHealthChange;
    }
    
    private void HandleHealthChange(int oldHealth, int newHealth)
    {
        _healthBarImage.fillAmount = (float)newHealth / health.MaxHealth;
    }
}
