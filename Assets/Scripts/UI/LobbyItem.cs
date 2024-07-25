using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text lobbyPlayersText;

    private LobbiesList _lobbiesList;
    private Lobby _lobby;

    public void Initialise(LobbiesList lobbiesList , Lobby lobby)
    {
        this._lobbiesList = lobbiesList;
        this._lobby = lobby;
        lobbyNameText.text = lobby.Name;
        lobbyPlayersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    public void Join()
    {
        _lobbiesList.JoinAsync(_lobby);
    }
}
