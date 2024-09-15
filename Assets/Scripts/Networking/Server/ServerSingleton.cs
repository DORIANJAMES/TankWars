
using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

public class ServerSingleton : MonoBehaviour
{
    private static ServerSingleton _instance;
    public ServerGameManager GameManager { get; private set; }

    public static ServerSingleton Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = FindObjectOfType<ServerSingleton>();
            if (_instance == null) 
            {
                Debug.LogError("There is no any server singleton in the scene.");
                return null;
            }

            return _instance;
        }
    }
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task CreateServer()
    {
        await UnityServices.InitializeAsync();
        GameManager = new ServerGameManager(
            ApplicationData.IP(),
            ApplicationData.Port(),
            ApplicationData.QPort(),
            NetworkManager.Singleton
            );
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
