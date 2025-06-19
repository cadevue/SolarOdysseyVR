using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

public class CoronalLoop : MonoBehaviour
{
    [Header("Emitter")]
    [SerializeField] private float _bound = 250f;
    [SerializeField] private VisualEffect _coronalLoopVFX;

    [Header("Coronal Loop Transform")]
    [SerializeField, OnValueChanged("RefreshPosition")] private float _emitterRadius = 1000f;
    [SerializeField, OnValueChanged("RefreshPosition")] private float _latitude = 0f;
    [SerializeField, OnValueChanged("RefreshPosition")] private float _longitude = 0f;
    [SerializeField, OnValueChanged("RefreshPosition")] private float _xRotation = 0f;
    [SerializeField, OnValueChanged("RefreshPosition")] private float _yRotation = 0f;
    [SerializeField, OnValueChanged("RefreshPosition")] private float _sinkDepth = 0f;

    [Header("Coronal Loop Shape")]
    [SerializeField, OnValueChanged("RefreshBezierPoints")] private float _footDistance = 64f;
    [SerializeField, OnValueChanged("RefreshBezierPoints")] private Vector3 _controlPointLeft = new Vector3(-32f, 64f, 0f);
    [SerializeField, OnValueChanged("RefreshBezierPoints")] private Vector3 _controlPointRight = new Vector3(32f, 64f, 0f);
    [SerializeField, OnValueChanged("RefreshCoronalLoopRadius")] private AnimationCurve _xRadiusOverT;
    [SerializeField, OnValueChanged("RefreshCoronalLoopRadius")] private AnimationCurve _yRadiusOverT;
    [SerializeField, OnValueChanged("RefreshCoronalLoopRadius")] private AnimationCurve _zRadiusOverT;
    [SerializeField, OnValueChanged("RefreshCoronalLoopRadius")] private float _peakRadius = 25f;

    [Header("Coronal Loop Plasma")]
    [SerializeField, OnValueChanged("RefreshPlasmaProperties")] private bool _spawnFromLeft = true;
    [SerializeField, OnValueChanged("RefreshPlasmaProperties")] private bool _spawnFromRight = true;
    [SerializeField, OnValueChanged("RefreshPlasmaProperties")] private float _plasmaSize = 2f;
    [SerializeField, OnValueChanged("RefreshPlasmaProperties")] private uint _plasmaSpawnRate = 12;
    [SerializeField, OnValueChanged("RefreshPlasmaProperties"), ColorUsage(true, true)] private Color _plasmaColor = Color.white;
    [SerializeField, OnValueChanged("RefreshPlasmaProperties")] private AnimationCurve _plasmaAlphaOverLife;
    [SerializeField, OnValueChanged("RefreshPlasmaProperties")] private AnimationCurve _plasmaSizeOverLife;
    [SerializeField, OnValueChanged("RefreshPlasmaProperties")] private Vector2 _plasmaLifetimeRange = new Vector2(1.5f, 3f);

    public void RefreshBezierPoints()
    {
        float halfFootDistance = _footDistance / 2f;
        Vector3 leftFoot = new Vector3(-halfFootDistance, 0f, 0f);
        Vector3 rightFoot = new Vector3(halfFootDistance, 0f, 0f);

        _coronalLoopVFX.SetVector3("BezierPoint_1", leftFoot);
        _coronalLoopVFX.SetVector3("BezierPoint_2", _controlPointLeft);
        _coronalLoopVFX.SetVector3("BezierPoint_3", _controlPointRight);
        _coronalLoopVFX.SetVector3("BezierPoint_4", rightFoot);
    }

    public void RefreshCoronalLoopRadius()
    {
        _coronalLoopVFX.SetAnimationCurve("XRadiusOverT", _xRadiusOverT);
        _coronalLoopVFX.SetAnimationCurve("YRadiusOverT", _yRadiusOverT);
        _coronalLoopVFX.SetAnimationCurve("ZRadiusOverT", _zRadiusOverT);
        _coronalLoopVFX.SetFloat("PeakRadius", _peakRadius);
    }

    public void RefreshPosition()
    {
        float latRad = _latitude * Mathf.Deg2Rad;
        float lonRad = _longitude * Mathf.Deg2Rad;

        float x = _emitterRadius * Mathf.Cos(latRad) * Mathf.Cos(lonRad);
        float y = _emitterRadius * Mathf.Sin(latRad);
        float z = _emitterRadius * Mathf.Cos(latRad) * Mathf.Sin(lonRad);

        Vector3 spherePos = new(x, y, z);
        Vector3 normal = spherePos.normalized;
        Vector3 sunkenPos = spherePos - normal * _sinkDepth;
        transform.position = sunkenPos;

        // Calculate base rotation: up = normal, forward = tangent
        Vector3 tangent = Vector3.ProjectOnPlane(Vector3.forward, normal).normalized;
        if (tangent == Vector3.zero) tangent = Vector3.ProjectOnPlane(Vector3.right, normal).normalized;
        Quaternion baseRot = Quaternion.LookRotation(tangent, normal);

        // Apply xRotation (around local X) and yRotation (around local Y)
        Quaternion offsetRot = Quaternion.Euler(_xRotation, _yRotation, 0f);
        transform.rotation = baseRot * offsetRot;
    }

    public void RefreshPlasmaProperties()
    {
        _coronalLoopVFX.SetUInt("LeftPlasmaSpawnRate", _spawnFromLeft ? _plasmaSpawnRate : 0);
        _coronalLoopVFX.SetUInt("RightPlasmaSpawnRate", _spawnFromRight ? _plasmaSpawnRate : 0);
        _coronalLoopVFX.SetFloat("PlasmaSize", _plasmaSize);
        _coronalLoopVFX.SetVector4("PlasmaColor", _plasmaColor);
        _coronalLoopVFX.SetAnimationCurve("PlasmaAlphaOverLife", _plasmaAlphaOverLife);
        _coronalLoopVFX.SetAnimationCurve("PlasmaSizeOverLife", _plasmaSizeOverLife);
        _coronalLoopVFX.SetVector2("PlasmaLifetimeRange", _plasmaLifetimeRange);
    }
}
