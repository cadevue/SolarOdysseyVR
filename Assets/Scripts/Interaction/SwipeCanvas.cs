using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

public class SwipeCanvas : MonoBehaviour
{
    [SerializeField] RectTransform canvasRect;
    [SerializeField] private float minimumRotationAngle = 15f;
    [SerializeField] private float facingThreshold = 0.6f;
    [SerializeField] private float upperFaceThreshold = 0.6f;

    private XRHandSubsystem m_HandSubsystem;

    private bool hasPrevious = false;
    private Vector3 lastPalmForward;
    private bool hasRotatedThisGesture = false;

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
        if (updateType != XRHandSubsystem.UpdateType.Dynamic)
            return;

        var rightHand = subsystem.rightHand;

        if (!rightHand.isTracked)
        {
            hasPrevious = false;
            hasRotatedThisGesture = false;
            return;
        }

        if (rightHand.GetJoint(XRHandJointID.Palm).TryGetPose(out Pose palmPose))
        {
            Vector3 palmForward = palmPose.rotation * Vector3.forward;
            Vector3 palmRight = palmPose.rotation * Vector3.right;

            float facingDot = Vector3.Dot(palmForward, canvasRect.forward);
            float upperDot = Vector3.Dot(palmForward, Vector3.up);

            if (facingDot < facingThreshold && upperDot < upperFaceThreshold)
            {
                hasPrevious = false;
                hasRotatedThisGesture = false;
                return;
            }

            Vector3 currentPalmForward = palmPose.forward;

            Vector3 flatPalmForward = Vector3.ProjectOnPlane(currentPalmForward, Vector3.up).normalized;

            if (hasPrevious)
            {
                float angleDelta = Vector3.SignedAngle(lastPalmForward, flatPalmForward, Vector3.up);

                if (!hasRotatedThisGesture && Mathf.Abs(angleDelta) >= minimumRotationAngle)
                {
                    float direction = Mathf.Sign(angleDelta);
                    canvasRect.anchoredPosition = canvasRect.anchoredPosition + new Vector2(750f * direction, 0f);
                    //canvasRect.DOAnchorPosX(750f * direction, 0.3f);
                    hasRotatedThisGesture = true;
                }

                if (Mathf.Abs(angleDelta) < 5f)
                {
                    hasRotatedThisGesture = false;
                }
            }

            lastPalmForward = flatPalmForward;
            hasPrevious = true;
        }
    }
}
