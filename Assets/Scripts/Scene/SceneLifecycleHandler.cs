using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DelayedEvent
{
    public float Delay;
    public UnityEvent Events;
}

public class SceneLifecycleHandler : MonoBehaviour
{
    [SerializeField] private UnityEvent _onAwake;
    [SerializeField] private UnityEvent _onStart;
    [SerializeField] private UnityEvent _onDestroy;
    [SerializeField] private DelayedEvent[] _delayedEvents;

    private void Awake()
    {
        _onAwake?.Invoke();
    }

    private void Start()
    {
        _onStart?.Invoke();
        if (_delayedEvents != null)
        {
            foreach (var delayedEvent in _delayedEvents)
            {
                if (delayedEvent != null && delayedEvent.Events != null)
                {
                    RunDelayedEvent(delayedEvent).Forget();
                }
            }
        }
    }

    private async UniTaskVoid RunDelayedEvent(DelayedEvent delayedEvent)
    {
        await UniTask.Delay((int)(delayedEvent.Delay * 1000f));
        delayedEvent.Events.Invoke();
    }

    private void OnDestroy()
    {
        _onDestroy?.Invoke();
    }

    public void PlaySound(string soundName)
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager instance is not available.");
            return;
        }
        AudioManager.Instance.Play(soundName);
    }

    public void PlaySoundOneShot(string soundName)
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager instance is not available.");
            return;
        }
        AudioManager.Instance.PlayOneShot(soundName);
    }

    public void StopSound(string soundName)
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager instance is not available.");
            return;
        }
        AudioManager.Instance.Stop(soundName);
    }
}