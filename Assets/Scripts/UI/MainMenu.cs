using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private TMP_Text playerPrefsName;
    [SerializeField] private TMP_Text findMatchStatusText;
    [SerializeField] private Texture2D cursorTexture;

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
