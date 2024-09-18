using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Map
{
    Default
}

public enum GameMode
{
    Default
}

public enum GameQueue
{
    Solo,
    Team
}

[Serializable]
public class UserData
{
    public string userName;
    public string userAuthId;
    public GameInfo userGamePreferences = new GameInfo();
}

[Serializable]
public class GameInfo
{
    public Map map;
    public GameMode gameMode;
    public GameQueue gameQueue;
    public string ToMultiplayQueue()
    {
        return gameQueue switch
        {
            GameQueue.Solo => "solo-que",
            GameQueue.Team => "team-que",
            _ => "solo-que"
        }; 
    }
}
