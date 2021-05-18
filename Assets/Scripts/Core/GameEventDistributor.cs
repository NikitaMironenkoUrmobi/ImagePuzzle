using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventDistributor
{
    public delegate void GameEvent();
    public static event GameEvent OnLevelStarted;
    public static event GameEvent OnLevelEnded;

    public static void CallStartLevel()
    {
        OnLevelStarted?.Invoke();
    }

    public static void CallEndLevel()
    {
        OnLevelEnded?.Invoke();
    }
}
