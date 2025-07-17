using System;
using System.Collections.Generic;

public interface IEvent
{
}

public static class EventBus
{
    private static Dictionary<Type, Delegate> _eventListeners = new Dictionary<Type, Delegate>();

    public static void AddListener<T>(Action<T> listener) where T : IEvent
    {
        Type eventType = typeof(T);

        if (_eventListeners.TryGetValue(eventType, out var existingDelegate))
        {
            _eventListeners[eventType] = Delegate.Combine(existingDelegate, listener);
        }
        else
        {
            _eventListeners[eventType] = listener;
        }
    }

    public static void RemoveListener<T>(Action<T> listener) where T : IEvent
    {
        Type eventType = typeof(T);

        if (_eventListeners.TryGetValue(eventType, out var existingDelegate))
        {
            var newDelegate = Delegate.Remove(existingDelegate, listener);
            if (newDelegate == null)
            {
                _eventListeners.Remove(eventType);
            }
            else
            {
                _eventListeners[eventType] = newDelegate;
            }
        }
    }

    public static void Invoke<T>(T eventData) where T : IEvent
    {
        Type eventType = typeof(T);

        if (_eventListeners.TryGetValue(eventType, out var existingDelegate))
        {
            var action = existingDelegate as Action<T>;
            action?.Invoke(eventData);
        }
    }
}