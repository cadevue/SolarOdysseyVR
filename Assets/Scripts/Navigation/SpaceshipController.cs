using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    public static SpaceshipController Instance { get; private set; }

    public enum TravelMode { Manual, Spline, Teleport }

    [Header("Speed Settings")]
    public float manualSpeed = 5f;
    public float splineSpeed = 3f;
    public float rotationSpeed = 60f;

    [Header("Status")]
    public TravelMode currentMode = TravelMode.Manual;
    public bool isMoving = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public float GetCurrentSpeed()
    {
        return currentMode switch
        {
            TravelMode.Manual => manualSpeed,
            TravelMode.Spline => splineSpeed,
            _ => 0f
        };
    }

    public void SwitchMode(TravelMode mode)
    {
        currentMode = mode;
    }

    public void SetMoving(bool moving)
    {
        isMoving = moving;
    }
}