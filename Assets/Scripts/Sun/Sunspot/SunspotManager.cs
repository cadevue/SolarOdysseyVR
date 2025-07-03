using System;
using NaughtyAttributes;
using UnityEngine;

public class SunspotManager : MonoBehaviour
{
    [SerializeField] private Material _sunspotMaterial;
    [SerializeField] private int _maxSunspots = 20;
    [SerializeField, Range(0, 1), OnValueChanged("UpdateSunspots")] private float _solarCycleProgress = 0.5f; // 0 = minimum, 1 = maximum
    [SerializeField] private Vector2 _sunspotSizeRange = new Vector2(0.02f, 0.1f);
    [SerializeField] private Vector2 _longitudeRange = new Vector2(135f, 225f);
    [SerializeField] private AnimationCurve _latitudeLineLocationCurve = AnimationCurve.Linear(0, 30, 1, 15);
    [SerializeField] private float _latitudeDeviation = 2.5f;
    [SerializeField] private Texture2DArray _sunspotTexArray;
    [SerializeField] private int[] _sunspotTexArrayWeights;
    [SerializeField, ReadOnly] private SunspotShaderData[] _sunspotsShaderData;
    [SerializeField, ReadOnly] private SunspotMemory[] _sunspotsMemory;

    [Serializable]
    struct SunspotShaderData
    {
        public Vector2 uv;
        public float rotation;
        public float scale;
        public int layer;
    }

    [Serializable]
    struct SunspotMemory
    {
        public int hemisphere; // 1 for northern, -1 for southern
        public float scale;
        public float longitude;
        public float latDeviation;
    }

    private ComputeBuffer _sunspotBuffer;

    [Button("Serialize Sunspots Data")]
    public void InitializeSunspots()
    {
        _sunspotsShaderData = new SunspotShaderData[_maxSunspots];
        _sunspotsMemory = new SunspotMemory[_maxSunspots];

        for (int i = 0; i < _maxSunspots; i++)
        {
            int hemisphere = UnityEngine.Random.value < 0.5f ? -1 : 1;
            float scale = UnityEngine.Random.Range(_sunspotSizeRange.x, _sunspotSizeRange.y);
            float rotation = UnityEngine.Random.Range(0f, 2 * Mathf.PI);

            // Select a random layer from the texture array based on weights
            int totalWeight = 0;
            for (int j = 0; j < _sunspotTexArrayWeights.Length; j++)
            {
                totalWeight += _sunspotTexArrayWeights[j];
            }
            int randomWeight = UnityEngine.Random.Range(0, totalWeight);
            int layer = 0;
            for (int j = 0; j < _sunspotTexArrayWeights.Length; j++)
            {
                if (randomWeight < _sunspotTexArrayWeights[j])
                {
                    layer = j;
                    break;
                }
                randomWeight -= _sunspotTexArrayWeights[j];
            }

            float longitude = UnityEngine.Random.Range(_longitudeRange.x, _longitudeRange.y);
            float latDeviation = UnityEngine.Random.Range(-_latitudeDeviation, _latitudeDeviation);

            _sunspotsMemory[i] = new SunspotMemory
            {
                hemisphere = hemisphere,
                scale = scale / 100,
                longitude = longitude,
                latDeviation = latDeviation
            };

            _sunspotsShaderData[i] = new SunspotShaderData
            {
                uv = Vector2.zero,
                rotation = rotation,
                scale = 0f,
                layer = layer
            };
        }
    }

    private void Start()
    {
        _sunspotBuffer?.Release();
        _sunspotBuffer = new ComputeBuffer(_maxSunspots, sizeof(float) * 4 + sizeof(int));

        UpdateSunspots();
    }

    public void UpdateSunspots()
    {
        if (!Application.isPlaying) return;

        int sunspotCount = Mathf.FloorToInt(_maxSunspots * _solarCycleProgress);

        for (int i = 0; i < sunspotCount; i++)
        {
            float latLine = _latitudeLineLocationCurve.Evaluate(_solarCycleProgress);
            int hemisphere = _sunspotsMemory[i].hemisphere;
            float lat = hemisphere * (latLine + _sunspotsMemory[i].latDeviation);
            float lon = _sunspotsMemory[i].longitude;
            Vector2 uv = SphericalToUVCoord(lat, lon);

            float scale = _sunspotsMemory[i].scale;

            _sunspotsShaderData[i].uv = uv;
            _sunspotsShaderData[i].scale = scale;
        }

        for (int i = sunspotCount; i < _maxSunspots; i++)
        {
            _sunspotsShaderData[i].uv = Vector2.zero;
            _sunspotsShaderData[i].scale = 0f;
        }

        _sunspotBuffer.SetData(_sunspotsShaderData);
        _sunspotMaterial.SetBuffer("_SunspotBuffer", _sunspotBuffer);
    }

    private Vector2 SphericalToUVCoord(float latitude, float longitude)
    {
        float u = longitude / 360f;
        float v = 0.5f - Mathf.Sin(latitude * Mathf.Deg2Rad) / 2f;
        return new Vector2(u, v);
    }

    private void OnDestroy()
    {
        if (_sunspotBuffer != null)
        {
            _sunspotBuffer.Release();
            _sunspotBuffer = null;
        }
    }
}