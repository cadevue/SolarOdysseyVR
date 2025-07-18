using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR.Features.Extensions.PerformanceSettings;

public class OpenXRConfiguration : MonoBehaviour
{
    readonly List<XRDisplaySubsystem> xrDisplays = new();
    private float foveatedRenderingLevel = 0.25f;
    private float pendingFoveatedRenderingLevel;
    private CancellationTokenSource debounceCts;
    [SerializeField] private float debounceDelay = 0.2f; // Adjustable debounce delay in seconds

    private void Start()
    {
        SubsystemManager.GetSubsystems(xrDisplays);
        if (xrDisplays.Count >= 1)
        {
            xrDisplays[0].foveatedRenderingLevel = foveatedRenderingLevel;
            xrDisplays[0].foveatedRenderingFlags
                = XRDisplaySubsystem.FoveatedRenderingFlags.GazeAllowed;
        }

        XrPerformanceSettingsFeature.SetPerformanceLevelHint(PerformanceDomain.Gpu, PerformanceLevelHint.SustainedHigh);
    }

    public void SetFoveatedRenderingLevel(float level)
    {
        pendingFoveatedRenderingLevel = Mathf.Clamp01(level);
        
        debounceCts?.Cancel();
        debounceCts = new CancellationTokenSource();
        
        ApplyFoveatedRenderingLevelDebounced(debounceCts.Token).Forget();
    }

    private async UniTask ApplyFoveatedRenderingLevelDebounced(CancellationToken cancellationToken)
    {
        try
        {
            await UniTask.Delay((int)(debounceDelay * 1000), cancellationToken: cancellationToken);
            
            foveatedRenderingLevel = pendingFoveatedRenderingLevel;
            if (xrDisplays.Count >= 1)
            {
                xrDisplays[0].foveatedRenderingLevel = foveatedRenderingLevel;
            }
        }
        catch (System.OperationCanceledException)
        {
            // Task was cancelled, which is expected behavior for debouncing
        }
    }

    public void SetActiveFoveatedRendering(bool isActive)
    {
        if (xrDisplays.Count >= 1)
        {
            xrDisplays[0].foveatedRenderingLevel = isActive ? foveatedRenderingLevel : 0f;
        }
    }

    private void OnDestroy()
    {
        // Clean up cancellation token source
        debounceCts?.Cancel();
        debounceCts?.Dispose();
    }
}