using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

public class PinchZoom : MonoBehaviour
{
    public XRHandSubsystem m_HandSubsystem;
    public Camera zoomedCamera;
    public float zoomSpeed = 100f;

    [SerializeField] private float initialDistance = 0f;
    [SerializeField]  private bool canZoom = false;
    [SerializeField]  private bool isZooming = false;

    private void Start()
    {
        var handSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(handSubsystems);

        foreach (var handSubsystem in handSubsystems)
        {
            if (handSubsystem.running)
            {
                m_HandSubsystem = handSubsystem;
                break;
            }
        }
        if (m_HandSubsystem != null)
        {
            m_HandSubsystem.updatedHands += OnUpdatedHands;
        }
    }

    private void OnDestroy()
    {
        if (m_HandSubsystem != null)
        {
            m_HandSubsystem.updatedHands -= OnUpdatedHands;
        }
    }

    void OnUpdatedHands(XRHandSubsystem subsystem,
        XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
        XRHandSubsystem.UpdateType updateType)
    {
        if (updateType != XRHandSubsystem.UpdateType.Dynamic || zoomedCamera == null)
            return;

        if (!canZoom)
        {
            isZooming = false;
            return;
        }

        var leftHand = subsystem.leftHand;
        var rightHand = subsystem.rightHand;

        if (!leftHand.isTracked || !rightHand.isTracked)
        {
            isZooming = false;
            return;
        }

        if (!leftHand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out Pose leftThumbPose) ||
            !leftHand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose leftIndexPose) ||
            !rightHand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out Pose rightThumbPose) ||
            !rightHand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose rightIndexPose))
        {
            isZooming = false;
            return;
        }

        float leftPinchDist = Vector3.Distance(leftThumbPose.position, leftIndexPose.position);
        float rightPinchDist = Vector3.Distance(rightThumbPose.position, rightIndexPose.position);
        float pinchThreshold = 0.02f;

        Debug.Log($"Left Pinch Distance: {leftPinchDist}, Right Pinch Distance: {rightPinchDist}");

        bool isPinching = (leftPinchDist < pinchThreshold) && (rightPinchDist < pinchThreshold);

        if (isPinching)
        {
            Vector3 leftPinchPos = (leftThumbPose.position + leftIndexPose.position) * 0.5f;
            Vector3 rightPinchPos = (rightThumbPose.position + rightIndexPose.position) * 0.5f;

            float currentDistance = Vector3.Distance(leftPinchPos, rightPinchPos);

            if (isZooming)
            {
                float delta = currentDistance - initialDistance;
                Debug.Log($"Delta: {delta}");

                    if (delta > 0)
                    {
                        // Zoom in
                        zoomedCamera.fieldOfView -= delta * zoomSpeed;
                    }
                    else
                    {
                        // Zoom out
                        zoomedCamera.fieldOfView += -delta * zoomSpeed;
                    }
            }

            initialDistance = currentDistance;
            isZooming = true;
        }
        else
        {
            isZooming = false;
            zoomedCamera.fieldOfView = 60f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player entered the trigger zone, enabling pinch zoom.");
            canZoom = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player exited the trigger zone, disabling pinch zoom.");
            canZoom = false;
            isZooming = false;
            zoomedCamera.fieldOfView = 60f; // Reset to default FOV
        }   
    }
}
