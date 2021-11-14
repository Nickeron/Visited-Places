using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    private readonly List<GameEventListener> eventListeners = new List<GameEventListener>();

    public void Raise<T>(T parameter)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
        {
            eventListeners[i].OnEventRaised(parameter);
        }
    }

    public void RegisterListener(GameEventListener listener)
    {
        if (eventListeners.Contains(listener)) return;

        eventListeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener listener)
    {
        if (!eventListeners.Contains(listener)) return;

        eventListeners.Remove(listener);
    }
}