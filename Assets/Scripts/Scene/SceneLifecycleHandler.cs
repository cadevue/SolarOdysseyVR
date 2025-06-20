using UnityEngine;
using UnityEngine.Events;

public class SceneLifecycleHandler : MonoBehaviour
{
    [SerializeField] private UnityEvent _onAwake;
    [SerializeField] private UnityEvent _onStart;
    [SerializeField] private UnityEvent _onDestroy;

    private void Awake()
    {
        _onAwake?.Invoke();
    }

    private void Start()
    {
        _onStart?.Invoke();
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
