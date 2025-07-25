using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;

public class RotateObjectCamera : MonoBehaviour
{
    [SerializeField] private Transform targetObject;
    [SerializeField] private Camera objectCamera;

    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private float verticalLimit = 80f;

    [SerializeField] private float pinchThreshold = 0.03f;
    [SerializeField] private float moveThreshold = 0.001f;

    [SerializeField] private bool useSmoothing = true;
    [SerializeField] private float smoothingFactor = 5f;

    private XRHandSubsystem m_HandSubsystem;

    private Vector3 previousPinchPosition;
    private bool isRotating = false;

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
        if (updateType != XRHandSubsystem.UpdateType.Dynamic || targetObject == null || objectCamera == null)
            return;

        var rightHand = subsystem.rightHand;

        if (!rightHand.isTracked)
        {
            isRotating = false;
            return;
        }

        if (IsPinchingGesture(rightHand, out Vector3 pinchPosition))
        {
            if (!isRotating)
            {
                previousPinchPosition = pinchPosition;
                isRotating = true;
            }
            else
            {
                Vector3 deltaPosition = pinchPosition - previousPinchPosition;
                Debug.Log($"Delta Position: {deltaPosition}");

                if (deltaPosition.magnitude > moveThreshold)
                {
                    float horizontalRotation = deltaPosition.x * rotationSpeed;
                    float verticalRotation = deltaPosition.y * rotationSpeed;

                    objectCamera.transform.RotateAround(targetObject.position, objectCamera.transform.up, horizontalRotation);
                    objectCamera.transform.RotateAround(targetObject.position, objectCamera.transform.right, verticalRotation);
                }

                //previousPinchPosition = pinchPosition;
                
            }
        }
        else
        {
            isRotating = false;
        }
    }

    private bool IsPinchingGesture(XRHand rightHand, out Vector3 pinchPosition)
    {
        pinchPosition = Vector3.zero;

        if (!rightHand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out Pose thumbPose) ||
            !rightHand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose indexPose))
        {
            return false;
        }

        pinchPosition = (thumbPose.position + indexPose.position) / 2f;

        float pinchDistance = Vector3.Distance(thumbPose.position, indexPose.position);
        return pinchDistance < pinchThreshold;
    }
}