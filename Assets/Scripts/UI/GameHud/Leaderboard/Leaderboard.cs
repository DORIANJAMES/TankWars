using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab;
    [SerializeField] private int entitiesToDisplay;
    
    
    private NetworkList<LeaderboardEntityState> _leaderboardEntities;
    private List<LeaderboardEntityDisplay> _entityDisplays = new List<LeaderboardEntityDisplay>();

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
                if (!_entityDisplays.Any(x => x.ClientId == changeevent.Value.ClientId))
                {
                    LeaderboardEntityDisplay leaderboardEntity = Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);
                    leaderboardEntity.Initialise(changeevent.Value.ClientId, changeevent.Value.PlayerName, changeevent.Value.Coins);
                    _entityDisplays.Add(leaderboardEntity);
                }
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                LeaderboardEntityDisplay displayToRemove = _entityDisplays.FirstOrDefault(x => x.ClientId == changeevent.Value.ClientId);
                if (displayToRemove != null)
                {
                    displayToRemove.transform.SetParent(null);
                    Destroy(displayToRemove.gameObject);
                    _entityDisplays.Remove(displayToRemove);
                }
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                LeaderboardEntityDisplay displayToUpdate =
                    _entityDisplays.FirstOrDefault(x => x.ClientId == changeevent.Value.ClientId);
                if (displayToUpdate!=null)
                {
                    displayToUpdate.UpdateCoins(changeevent.Value.Coins);
                }
                break;
        }
        _entityDisplays.Sort((x,y) => y.Coins.CompareTo(x.Coins));
        for (int i = 0; i < _entityDisplays.Count; i++)
        {
            _entityDisplays[i].transform.SetSiblingIndex(i);
            _entityDisplays[i].UpdateText();
            _entityDisplays[i].gameObject.SetActive(i<=entitiesToDisplay-1);
        }

        LeaderboardEntityDisplay myDisplay = _entityDisplays.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);
        if (myDisplay != null)
        {
            if (myDisplay.transform.GetSiblingIndex() >= entitiesToDisplay)
            {
                leaderboardEntityHolder.GetChild(entitiesToDisplay-1).gameObject.SetActive(false);
                myDisplay.gameObject.SetActive(true);
            }
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

        player.Wallet.totalCoins.OnValueChanged += (oldCoins, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins);
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
        player.Wallet.totalCoins.OnValueChanged -= (oldCoins, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins);
    }

    private void HandleCoinsChanged(ulong clientId, int newCoins)
    {
        for (int i = 0; i < _leaderboardEntities.Count; i++)
        {
            if (_leaderboardEntities[i].ClientId != clientId) { continue; }

            _leaderboardEntities[i] = new LeaderboardEntityState
            {
                ClientId = _leaderboardEntities[i].ClientId,
                PlayerName = _leaderboardEntities[i].PlayerName,
                Coins = newCoins
            };
        }
        return;
    }
}
