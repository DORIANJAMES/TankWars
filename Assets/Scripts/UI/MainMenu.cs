using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private TMP_Text playerPrefsName;
    [SerializeField] private TMP_Text findMatchStatusText;
    [SerializeField] private TMP_Text findMatchStatusButtonText;
    [SerializeField] private Texture2D cursorTexture;
    private bool _isMatchMaking;
    private bool _isCancelling;

    private void Start()
    {
        if (ClientSingleton.Instance == null)
        {
            return;
        }

        Vector2 hotSpot = new Vector2(cursorTexture.width / 20, cursorTexture.height / 20);
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }

    private void OnEnable()
    {
        playerPrefsName.text = PlayerPrefs.GetString(NameSelector.PlayerNameKey, string.Empty);
    }

    public async void FindMatchPressed()
    {
        if (_isCancelling)
        {
            return;
        }
        if (_isMatchMaking)
        {
            findMatchStatusText.text = "Cancelling...!";
            _isCancelling = true;
            await ClientSingleton.Instance.GameManager.CancelMatchMaking();
            _isCancelling = false;
            _isMatchMaking = false;
            findMatchStatusButtonText.text = "Find Match";
            findMatchStatusText.text = playerPrefsName.text;
            return;
        }
        ClientSingleton.Instance.GameManager.MatchMakeAsync(OnMatchMade);
        findMatchStatusButtonText.text = "Cancel";
        findMatchStatusText.text = "Searching...!";
        _isMatchMaking = true;
        
    }

    private void OnMatchMade(MatchmakerPollingResult result)
    {
        switch (result)
        {
         case MatchmakerPollingResult.Success:
             findMatchStatusText.text = "Connecting...!";
             break;
         case MatchmakerPollingResult.TicketCreationError:
             findMatchStatusText.text = "Creation Error";
             break;
         case MatchmakerPollingResult.TicketCancellationError:
             findMatchStatusText.text = "Cannelation Error";
             break;
         case MatchmakerPollingResult.TicketRetrievalError:
             findMatchStatusText.text = "Retrieval Error";
             break;
         case MatchmakerPollingResult.MatchAssignmentError:
             findMatchStatusText.text = "Assignment Error";
             break;
         default:
             break;
        }
    }

    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }
    
    public async void StartClient()
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
    }

    public void BackAction()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }
}
