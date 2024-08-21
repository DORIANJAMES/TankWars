using System;
using Unity.Netcode;
using Cinemachine;
using Core.Combat;
using Unity.Collections;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{

    [Header("References")] 
    [SerializeField] private SpriteRenderer miniMapSpriteRenderer;
    [SerializeField] public CinemachineVirtualCamera cinemachineVirtualCamera;
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public CoinWallet Wallet { get; private set; }
    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    [Header("Settings")] 
    [SerializeField] private Color miniMapSpriteColor;
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
            Vector3 miniMapSpriteRendederScale = miniMapSpriteRenderer.transform.localScale;
            CinemachineVirtualCamera cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Priority = 15;
            miniMapSpriteRenderer.color = miniMapSpriteColor;
            miniMapSpriteRenderer.transform.localScale = miniMapSpriteRendederScale * 2;
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
