using System;
using UnityEngine;

[Serializable]
public enum Hemisphere
{
    Northern = 1,
    Southern = -1
}

[Serializable]
public struct SunspotBufferData
{
    public Vector2 UV_Position;
    public float Rotation;
    public float Scale;
    public int Layer;
}

[Serializable]
public struct SunspotLocation
{
    public Hemisphere Hemisphere;
    public float Longitude;
    public float DistanceFromLatitude;
}

[Serializable]
public class SunspotModel
{
    public SunspotBufferData[] Sunspots;
    public SunspotLocation[] Locations;
}