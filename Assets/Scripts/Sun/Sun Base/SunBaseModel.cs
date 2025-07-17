using System;
using UnityEngine;

[Serializable]
public class SunBaseModel
{
    [ColorUsage(true, true)]
    public Color SunColor;

    public float NoiseTextureScalOne;
    public float NoiseTextureScaleTwo;

    public Vector3 DistortionSpeed;

    [ColorUsage(true, true)]
    public Color FresnelColor;
    public float FresnelPower;
    public float FresnelNoiseScale;
}