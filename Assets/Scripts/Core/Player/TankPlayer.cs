using System;
using Unity.Netcode;
using Cinemachine;
using Core.Combat;
using Unity.Collections;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [SerializeField] public CinemachineVirtualCamera cinemachineVirtualCamera;
    [field: SerializeField] public Health Health { get; private set; }
    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawn;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData = HostSingleton.Instance.GameManager.networkServer.GetUserDataByClientId(OwnerClientId);
            playerName.Value = userData.userName;
            
            OnPlayerSpawned?.Invoke(this);
        }
        if (IsOwner)
        {
            CinemachineVirtualCamera cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Priority = 15;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawn?.Invoke(this);
        }
    }
   
}
