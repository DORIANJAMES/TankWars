using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
    [SerializeField] private const string MenuSceneName = "Menu";
    public async Task<bool> InitAsync()
    {
        // Authenticate Player
        await UnityServices.InitializeAsync();
        AuthState authState = await AuthenticationWrapper.DoAuth();

        if (authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;

    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }
}
