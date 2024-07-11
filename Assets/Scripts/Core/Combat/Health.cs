using System;
using Unity.Netcode;
using UnityEngine;

namespace Core.Combat
{
   public class Health : NetworkBehaviour
   {
      [field: SerializeField] public int MaxHealth { get; private set; } = 100;
      public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

      public bool isDead;
      public Action<Health> OnDeath;
      
      public override void OnNetworkSpawn()
      {
         if (!IsServer)
            return;
         
         currentHealth.Value = MaxHealth;
      }

      public void TakeDamage(int damagaValue)
      {
         ModifyHealth(-damagaValue);
      }
      
      public void RestoreDamage(int healValue)
      {
         ModifyHealth(healValue);
      }
      
      private void ModifyHealth(int value)
      {
         if (isDead)
            return;
         
         int newHealth = currentHealth.Value + value;
         currentHealth.Value = Mathf.Clamp(newHealth, 0, MaxHealth);

         switch (currentHealth.Value)
         {
            case<= 0:
               OnDeath?.Invoke(this);
               isDead = true;
               Debug.Log("Dead :D");
               break;
            default:
               break;
         }
      }
   }
}
