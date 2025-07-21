using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class PageEvents
{
    [Header("Page Events")]
    public UnityEvent onPageActivated;
    public UnityEvent onPageDeactivated;
    public UnityEvent onPageShown;
    public UnityEvent onPageHidden;
}

public class PageEventComponent : MonoBehaviour
{
    [Header("Events")]
    public PageEvents events;

    private float activationDelay = 3f;
    private Coroutine currentCoroutine;

    public void OnPageActivated()
    {
        if (activationDelay > 0)
        {
            Invoke(nameof(TriggerActivation), activationDelay);
        }
        else
        {
            TriggerActivation();
        }
    }

    public void OnPageDeactivated()
    {
        if (activationDelay > 0)
        {
            Invoke(nameof(TriggerDeactivation), activationDelay);
        }
        else
        {
            TriggerDeactivation();
        }
    }

    public void OnPageShown()
    {
        if (activationDelay > 0)
        {
            Invoke(nameof(TriggerShown), activationDelay);
        }
        else
        {
            TriggerShown();
        }
    }

    public void OnPageHidden()
    {
        if (activationDelay > 0)
        {
            Invoke(nameof(TriggerHidden), activationDelay);
        }
        else
        {
            TriggerHidden();
        }
    }

    private void TriggerActivation()
    {
        events.onPageActivated?.Invoke();
    }

    private void TriggerDeactivation()
    {
        events.onPageDeactivated?.Invoke();
    }

    private void TriggerShown()
    {
        events.onPageShown?.Invoke();
    }

    private void TriggerHidden()
    {
        events.onPageHidden?.Invoke();
    }
}