using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper 
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        if (AuthState == AuthState.Authenticated)
        {
            return AuthState;
        }

        if (AuthState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already Authenticating");
            await Authenticating();
            return AuthState;
        }

        await SignInAnonymouslyAsync(maxTries);
        
        return AuthState;
    }

    private static async Task<AuthState> Authenticating()
    {
        while (AuthState == AuthState.Authenticated || AuthState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }
        return AuthState;
    }

    private static async Task SignInAnonymouslyAsync(int maxTries)
    {
        AuthState = AuthState.Authenticating;
        int tries = 0;
        while (AuthState == AuthState.Authenticating && tries < maxTries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException authException)
            {
                Debug.LogError(authException);
                AuthState = AuthState.Error;
            }
            catch (RequestFailedException requestFailedException)
            {
                Debug.LogError(requestFailedException);
                AuthState = AuthState.Error;
            }
            tries++;
            await Task.Delay(1000);
        }

        if (AuthState != AuthState.Authenticated)
        {
            Debug.LogWarning($"Player was not signed in successfully after {tries} tries");
            AuthState = AuthState.TimeOut;
        }
    }
}

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}
