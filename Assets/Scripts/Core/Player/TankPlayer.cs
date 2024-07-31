using Unity.Netcode;
using Cinemachine;
using Unity.Collections;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [SerializeField] public CinemachineVirtualCamera cinemachineVirtualCamera;
    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData = HostSingleton.Instance.GameManager.networkServer.GetUserDataByClientId(OwnerClientId);
            playerName.Value = userData.userName;
        }
        if (IsOwner)
        {
            CinemachineVirtualCamera cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Priority = 15;
        }
    }

   
}
