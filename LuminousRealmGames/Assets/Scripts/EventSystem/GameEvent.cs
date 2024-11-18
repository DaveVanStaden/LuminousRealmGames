using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Scriptable Objects/GameEvent")]
public class GameEvent : ScriptableObject
{

    public List<GameEventListener> listeners = new();
    
    public void Trigger()
    {
        for (int i = 0; i < listeners.Count; i++)
        {
            listeners[i].Trigger();
        }
    }

    public void AddListener(GameEventListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void RemoveListener(GameEventListener listener)
    {
        listeners.Remove(listener);
    }
}
