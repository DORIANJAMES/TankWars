using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private GameObject QuitPanel;

    private void OnEnable()
    {
        QuitPanel.SetActive(false);
    }

    public void OpenTheQuitPanel()
    {
        QuitPanel.SetActive(true);
    }

    public void CloseTheQuitPanel()
    {
        QuitPanel.SetActive(false);
    }

    public void LeaveGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.GameManager.ShutDown();
        }

        ClientSingleton.Instance.GameManager.Disconnect();
    }
}
