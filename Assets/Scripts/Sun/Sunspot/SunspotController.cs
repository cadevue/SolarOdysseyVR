using NaughtyAttributes;
using UnityEngine;

public class SunspotController : MonoBehaviour
{
    [Header("Sunpot Settings")]
    [SerializeField]
    private int _maxSunspots = 20;
    [SerializeField]
    private Vector2 _sunspotSizeRange = new Vector2(0.02f, 0.1f);
    [SerializeField]
    private float _activeLongitude = 30f;
    [SerializeField]
    private float _longitudeDeviation = 20f;
    [SerializeField]
    private AnimationCurve _latitudeCenterOverSolarCycle = AnimationCurve.Linear(0, 30, 1, 5);
    [SerializeField]
    private float _sunspotLatitudeMaxDeviation = 2.5f;
    [SerializeField]
    private int _sunspotTexArraySize = 8;

    [Header("Serializable Data")]
    [SerializeField, ReadOnly]
    private SunspotModel _sunspotModel;
    [SerializeField]
    private SunspotView _sunspotView;

    private void Start()
    {
        _sunspotView.Init();
    }

    [Button]
    public void SerializeSunspots()
    {
        _sunspotModel = new SunspotModel
        {
            Sunspots = new SunspotBufferData[_maxSunspots],
            Locations = new SunspotLocation[_maxSunspots]
        };

        for (int i = 0; i < _maxSunspots; i++)
        {
            // Randomize Scale
            float scale = Random.Range(_sunspotSizeRange.x, _sunspotSizeRange.y);

            // Randomize Rotation
            float rotation = Random.Range(0f, 2 * Mathf.PI);

            // Randomize Longitude
            float longitudeRegionCenter = Random.value < 0.5f ? _activeLongitude : _activeLongitude + 180f;
            float longitudeDeviation = Random.Range(-_longitudeDeviation, _longitudeDeviation);
            float longitude = longitudeRegionCenter + longitudeDeviation;

            // Randomize Latitude
            float latitudeDeviation = Random.Range(-_sunspotLatitudeMaxDeviation, _sunspotLatitudeMaxDeviation);
            int hemisphere = Random.value < 0.5f ? (int)Hemisphere.Southern : (int)Hemisphere.Northern;
            float latitude = _latitudeCenterOverSolarCycle.Evaluate(Random.value) * hemisphere + latitudeDeviation;

            // Store data in Model
            _sunspotModel.Sunspots[i] = new SunspotBufferData
            {
                UV_Position = SphericalToUVCoord(latitude, longitude),
                Rotation = rotation,
                Scale = scale / 100,
                Layer = Random.Range(0, _sunspotTexArraySize)
            };

            _sunspotModel.Locations[i] = new SunspotLocation
            {
                Hemisphere = (Hemisphere)hemisphere,
                Longitude = longitude,
                DistanceFromLatitude = latitudeDeviation
            };
        }
    }

    public void UpdateSunspotBasedOnSolarCycle(float solarCycleProgress)
    {
    }

    private Vector2 SphericalToUVCoord(float latitude, float longitude)
    {
        float u = longitude / 360f;
        float v = 0.5f - Mathf.Sin(latitude * Mathf.Deg2Rad) / 2f;
        return new Vector2(u, v);
    }
}