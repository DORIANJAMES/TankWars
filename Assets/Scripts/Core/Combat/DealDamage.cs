using System;
using System.Collections;
using System.Collections.Generic;
using Core.Combat;
using Unity.Netcode;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField] private int damage;
    private ulong ownerClientId;

    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody == null)
            return;
        if (other.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            Debug.Log("Owner Client ID = "+netObj.OwnerClientId);
            Debug.Log("isOwner = "+netObj.IsOwner);
            Debug.Log("Get Observers Hash = "+netObj.GetObservers());
            Debug.Log("Get Hash Code = "+netObj.GetHashCode());
            
            if (ownerClientId == netObj.OwnerClientId)
                return;
        }
        if (other.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }
    }

    
}
