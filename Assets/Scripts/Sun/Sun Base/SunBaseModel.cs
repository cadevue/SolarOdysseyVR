using System;
using UnityEngine;

[Serializable]
public class SunBaseModel
{
    [ColorUsage(true, true)]
    public Color SunColor;

    public float NoiseTextureScaleOne;
    public float NoiseTextureScaleTwo;

    public Vector3 SurfaceDistortionSpeed;

    [ColorUsage(true, true)]
    public Color FresnelColor;
    public float FresnelPower;
    public float FresnelNoiseScale;
}