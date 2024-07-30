using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button connectButton;
    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private int minNameLength = 1;
    [SerializeField] private int maxNameLength = 25;

    public const string PlayerNameKey = "PlayerKey";
    private void Start()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }
        nameField.text = PlayerPrefs.GetString(PlayerNameKey, string.Empty);
        HandleNameChanged();
    }

    public void HandleNameChanged()
    {
        connectButton.interactable = nameField.text.Length >= minNameLength && nameField.text.Length <= maxNameLength;
        nameDisplay.text = nameField.text;
    }

    public void Connect()
    {
        PlayerPrefs.SetString(PlayerNameKey, nameDisplay.text);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
