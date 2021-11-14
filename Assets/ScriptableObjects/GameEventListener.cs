using UnityEngine;

public abstract class GameEventListener : MonoBehaviour
{
    [Tooltip("Event to register with")]
    public GameEvent Event;

    protected void OnEnable()
    {
        Event.RegisterListener(this);
    }

    protected void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public abstract void OnEventRaised<T>(T parameter);
}
