using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

public class SwipeCanvas : MonoBehaviour
{
    [SerializeField] GameObject canvasObject;
    // [SerializeField] RectTransform canvasRect;
    [SerializeField] GuideUI guideUI;
    // [SerializeField] private float deltaPosition = 750f;
    [SerializeField] private float minimumRotationAngle = 10f;
    [SerializeField] private float facingThreshold = 0.7f;
    [SerializeField] private float upperFaceThreshold = 0.6f;

    private XRHandSubsystem m_HandSubsystem;

    private bool hasPrevious = false;
    private Vector3 lastPalmForward;
    private bool hasRotatedThisGesture = false;
    [SerializeField] private int currentPageIndex = 0;

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

        if (guideUI == null)
        {
            guideUI = FindObjectOfType<GuideUI>();
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

            float facingDot = palmForward.z;
            float upperDot = Vector3.Dot(palmForward, Vector3.up);

            if (facingDot < facingThreshold)
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
                    float direction = Mathf.Sign(angleDelta) * -1f;
                    Debug.Log($"Turning page with direction: {direction}, angleDelta: {angleDelta}");

                    if (guideUI != null && guideUI.IsVisible)
                    {
                        TurnPage(direction);
                    }
                    // canvasRect.anchoredPosition = canvasRect.anchoredPosition + new Vector2(deltaPosition * direction, 0f);
                    // //canvasRect.DOAnchorPosX(750f * direction, 0.3f);
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

    private void TurnPage(float direction)
    {
        if (guideUI == null || guideUI.ActivePagesCount == 0) return;

        guideUI.TurnPage(direction);
    }
}
