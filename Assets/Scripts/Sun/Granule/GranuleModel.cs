using System;
using UnityEngine;

[Serializable]
public class GranuleModel
{
    public Gradient GranuleColor;
    public float GranuleOffsetSpeed;

    [Range(0, 12)]
    public uint GranuleQuantizeStep;

    public Vector2 GranuleTilingSize;

    [Range(0, 1)]
    public float GranuleVisibility;

}