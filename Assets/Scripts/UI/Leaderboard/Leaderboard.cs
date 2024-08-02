using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntityDispaly leaderboardEntityPrefab;
    private NetworkList<LeaderboardEntityState> _leaderboardEntities;

    private void Awake()
    {
        _leaderboardEntities = new NetworkList<LeaderboardEntityState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            _leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;
            foreach (LeaderboardEntityState entity in _leaderboardEntities)
            {
                HandleLeaderboardEntitiesChanged(new NetworkListEvent<LeaderboardEntityState>
                {
                    Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                    Value = entity,
                });
            }
        }
        if (!IsServer) { return; }
        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        foreach (TankPlayer player in players)
        {
            HandlePlayerSpawned(player);
        }

        TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawn += HandlePlayerDespawned;
    }
    

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            _leaderboardEntities.OnListChanged -= HandleLeaderboardEntitiesChanged;
        }
        if (!IsServer) { return; }
        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawn -= HandlePlayerDespawned;
        
    }
    
    private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntityState> changeevent)
    {
        switch (changeevent.Type)
        {
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                break;
        }
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        _leaderboardEntities.Add( new LeaderboardEntityState
        {
            ClientId = player.OwnerClientId,
            PlayerName = player.playerName.Value,
            Coins = 0
        });
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        if (_leaderboardEntities==null) { return; }
        foreach (LeaderboardEntityState entity in _leaderboardEntities)
        {
            if (entity.ClientId != player.OwnerClientId) { continue; }
            _leaderboardEntities.Remove(entity);
            break;
        }
    }
}
