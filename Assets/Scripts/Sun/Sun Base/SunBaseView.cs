using UnityEngine;

public class SunBaseView : MonoBehaviour
{
    [SerializeField]
    private Material _sunMaterial;

    public void SetView(SunBaseModel model)
    {
        _sunMaterial.SetColor("_SunColor", model.SunColor);
        _sunMaterial.SetFloat("_NoiseTextureScaleOne", model.NoiseTextureScaleOne);
        _sunMaterial.SetFloat("_NoiseTextureScaleTwo", model.NoiseTextureScaleTwo);
        _sunMaterial.SetVector("_SurfaceDistortionSpeed", model.SurfaceDistortionSpeed);
        _sunMaterial.SetColor("_FresnelColor", model.FresnelColor);
        _sunMaterial.SetFloat("_FresnelPower", model.FresnelPower);
        _sunMaterial.SetFloat("_FresnelNoiseScale", model.FresnelNoiseScale);
    }
}