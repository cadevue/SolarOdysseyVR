using LitMotion;
using UnityEngine;

public class GranuleManager : MonoBehaviour
{
    [SerializeField] private Material _granuleMaterial;
    [SerializeField] private float _granuleAnimationDuration = 0.5f;
    [SerializeField] private Ease _granuleAnimationEase = Ease.OutQuad;
    private MotionHandle _granuleAnimationHandle;

    public void SetShowGranules(bool isActive)
    {
        if (_granuleAnimationHandle.IsActive())
        {
            _granuleAnimationHandle.Cancel();
        }

        float currentValue = _granuleMaterial.GetFloat("_GranuleView");
        float targetValue = isActive ? 1f : 0f;
        _granuleAnimationHandle = LMotion.Create(currentValue, targetValue, _granuleAnimationDuration)
            .WithEase(_granuleAnimationEase)
            .Bind(value => _granuleMaterial.SetFloat("_GranuleView", value));
    }
}