using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

public class DimmerUI : MonoBehaviour
{
    private CanvasGroup _dimmerImage;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private Ease _fadeEase = Ease.Linear;

    private void Awake()
    {
        _dimmerImage = GetComponent<CanvasGroup>();
    }

    public void SetDuration(float duration)
    {
        _fadeDuration = duration;
    }

    public void SetEase(Ease ease)
    {
        _fadeEase = ease;
    }

    public void OpaqueToTransparent()
    {
        LMotion.Create(_dimmerImage.alpha, 0f, _fadeDuration)
            .WithEase(_fadeEase)
            .BindToAlpha(_dimmerImage);
    }

    public void TransparentToOpaque()
    {
        LMotion.Create(_dimmerImage.alpha, 1f, _fadeDuration)
            .WithEase(_fadeEase)
            .BindToAlpha(_dimmerImage);
    }
}
