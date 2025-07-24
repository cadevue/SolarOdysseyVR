using UnityEngine;

public class CopyCameraMove : MonoBehaviour
{
    [SerializeField] Transform playerCamera;
    [SerializeField] Transform anchorA;
    [SerializeField] Transform anchorB;
    
    [Header("Performance Settings")]
    [SerializeField] bool useFixedUpdate = false;
    
    [Header("VR Stability Settings")]
    [SerializeField] bool enableSmoothing = true;
    [SerializeField] float smoothingFactor = 0.1f; // Lower = smoother, Higher = more responsive
    [SerializeField] bool enableDeadzone = true;
    [SerializeField] float positionDeadzone = 0.001f; // Minimum movement threshold
    [SerializeField] float rotationDeadzone = 0.1f; // Minimum rotation threshold in degrees
    
    // Cache for performance and smoothing
    private Matrix4x4 transformMatrix;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private bool isInitialized = false;

    private void Awake()
    {
        float distA = Vector3.Distance(anchorA.position, playerCamera.position);
        float distB = Vector3.Distance(anchorB.position, transform.position);

        float scaleFactor = distA / distB;
        anchorA.localScale = Vector3.one * scaleFactor;
        
        // Initialize smoothing values
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        targetPosition = lastPosition;
        targetRotation = lastRotation;
    }

    void Update()
    {
        // Use Update instead of LateUpdate for immediate response
        if (!useFixedUpdate)
        {
            UpdateCameraTransform();
        }
    }

    void FixedUpdate()
    {
        if (useFixedUpdate)
        {
            UpdateCameraTransform();
        }
    }

    private void UpdateCameraTransform()
    {
        if (playerCamera == null || anchorA == null || anchorB == null) return;

        var m = anchorB.localToWorldMatrix * anchorA.worldToLocalMatrix * playerCamera.localToWorldMatrix;
        
        // Extract position and rotation from matrix
        Vector3 newPosition = m.GetColumn(3);
        Quaternion newRotation = ExtractRotation(m);
        
        // Initialize if first run
        if (!isInitialized)
        {
            lastPosition = newPosition;
            lastRotation = newRotation;
            targetPosition = newPosition;
            targetRotation = newRotation;
            isInitialized = true;
        }
        
        // Apply deadzone filtering
        if (enableDeadzone)
        {
            // Position deadzone
            float positionDelta = Vector3.Distance(newPosition, lastPosition);
            if (positionDelta < positionDeadzone)
            {
                newPosition = lastPosition;
            }
            
            // Rotation deadzone
            float rotationDelta = Quaternion.Angle(newRotation, lastRotation);
            if (rotationDelta < rotationDeadzone)
            {
                newRotation = lastRotation;
            }
        }
        
        // Update targets
        targetPosition = newPosition;
        targetRotation = newRotation;
        
        // Apply smoothing
        if (enableSmoothing)
        {
            // Use Time.unscaledDeltaTime for consistent smoothing regardless of time scale
            float deltaTime = useFixedUpdate ? Time.fixedUnscaledDeltaTime : Time.unscaledDeltaTime;
            float smoothing = Mathf.Clamp01(smoothingFactor / deltaTime);
            
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothing);
            Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothing);
            
            transform.SetPositionAndRotation(smoothedPosition, smoothedRotation);
        }
        else
        {
            // Apply transformation immediately without smoothing
            transform.SetPositionAndRotation(targetPosition, targetRotation);
        }
        
        // Update last values for next frame
        lastPosition = targetPosition;
        lastRotation = targetRotation;
    }

    private Quaternion ExtractRotation(Matrix4x4 m)
    {
        Vector3 forward = m.GetColumn(2).normalized;
        Vector3 upwards = m.GetColumn(1).normalized;

        Vector3 right = Vector3.Cross(upwards, forward).normalized;
        upwards = Vector3.Cross(forward, right).normalized;

        return Quaternion.LookRotation(forward, upwards);
    }
}
