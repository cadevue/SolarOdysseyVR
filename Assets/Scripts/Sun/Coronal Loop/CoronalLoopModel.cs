using System;
using UnityEngine;

[Serializable]
public class CoronalLoopModel
{
    public Vector3 BezierPointOne;
    public Vector3 BezierPointTwo;
    public Vector3 BezierPointThree;
    public Vector3 BezierPointFour;

    public AnimationCurve XRadiusOverLife;
    public AnimationCurve YRadiusOverLife;
    public AnimationCurve ZRadiusOverLife;

    public float PeakRadius;

    public uint LeftPlasmaSpawnRate;
    public uint RightPlasmaSpawnRate;

    public Color PlasmaColor;
    public float PlasmaSize;
    public AnimationCurve PlasmaAlphaOverLife;
    public AnimationCurve PlasmaSizeOverLife;
    public Vector2 PlasmaLifetimeRange;
}