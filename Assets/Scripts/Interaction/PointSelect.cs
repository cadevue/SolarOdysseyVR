// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.XR.Hands;

// public class PointSelect : MonoBehaviour
// {
//     [Header("Settings")]
//     [SerializeField] private float maxRayDistance = 10f;
//     [SerializeField] private float selectionTime = 2f;
//     [SerializeField] private LayerMask interactableLayer = -1;

//     [Header("Visual Feedback")]
//     [SerializeField] private LineRenderer pointerLine;
//     [SerializeField] private GameObject selectionIndicator;
//     [SerializeField] private Transform selectionProgressRing;

//     [Header("Events")]
//     public UnityEvent<GameObject> onObjectSelected;
//     public UnityEvent<GameObject> onObjectHovered;
//     public UnityEvent onSelectionCancelled;

//     private XRHandSubsystem handSubsystem;
//     private GameObject currentTarget;
//     private float selectionTimer = 0f;
//     private bool isSelecting = false;

//     private void Start()
//     {
//         var handSubsystems = new List<XRHandSubsystem>();
//         SubsystemManager.GetSubsystems(handSubsystems);

//         foreach (var subsystem in handSubsystems)
//         {
//             if (subsystem.running)
//             {
//                 handSubsystem = subsystem;
//                 break;
//             }
//         }

//         if (handSubsystem != null)
//         {
//             handSubsystem.updatedHands += OnUpdatedHands;
//         }
//     }

//     private void OnDestroy()
//     {
//         if (handSubsystem != null)
//         {
//             handSubsystem.updatedHands -= OnUpdatedHands;
//         }
//     }

//     void OnUpdatedHands(XRHandSubsystem subsystem,
//         XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
//         XRHandSubsystem.UpdateType updateType)
//     {
//         if (updateType != XRHandSubsystem.UpdateType.Dynamic || pointerLine == null)
//             return;

//         var rightHand = subsystem.rightHand;

//         if (!rightHand.isTracked)
//         {
//             DisablePointing();
//             return;
//         }

//         if (IsPointingGesture(rightHand))
//         {
//             EnablePointing();
//             HandlePointing(rightHand);
//         }
//         else
//         {
//             DisablePointing();
//         }

//         Vector3 rayOrigin = rightHand.palmPosition;
//         Vector3 rayDirection = rightHand.palmForward;

//         RaycastHit hit;
//         if (Physics.Raycast(rayOrigin, rayDirection, out hit, maxRayDistance, interactableLayer))
//         {
//             HandleSelection(hit.collider.gameObject);
//         }
//         else
//         {
//             ResetSelection();
//         }

//         UpdatePointerVisuals(rayOrigin, rayDirection);
//     }

//     private void EnablePointing()
//     {
//         pointerLine.enabled = true;
//         selectionIndicator.SetActive(true);
//     }

//     private void DisablePointing()
//     {
//         pointerLine.enabled = false;
//         selectionIndicator.SetActive(false);
//         ResetSelection();
//     }

//     private bool IsPointingGesture(XRHand rightHand)
//     {
//         if (!hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose indexTip) ||
//             !hand.GetJoint(XRHandJointID.IndexProximal).TryGetPose(out Pose indexProximal) ||
//             !hand.GetJoint(XRHandJointID.MiddleTip).TryGetPose(out Pose middleTip) ||
//             !hand.GetJoint(XRHandJointID.MiddleProximal).TryGetPose(out Pose middleProximal))
//         {
//             return false;
//         }

//         float indexExtension = Vector3.Distance(indexTip.position, indexProximal.position);
//         float middleExtension = Vector3.Distance(middleTip.position, middleProximal.position);

//         return indexExtension > middleExtension * 1.2f;
//     }

//     private void HandlePointing(XRHand rightHand)
//     {
//         if (rightHand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose indexTipPose))
//         {
//             Vector3 rayOrigin = indexTipPose.position;
//             Vector3 rayDirection = indexTipPose.forward;

//             PeformRaycast(rayOrigin, rayDirection);
//             UpdateVisuals(rayOrigin, rayDirection);
//             isSelecting = true;
//         }
//         else
//         {
//             DisablePointing();
//         }
//         if (currentTarget != null && !isSelecting)
//         {
//             selectionTimer += Time.deltaTime;
//             if (selectionTimer >= selectionTime)
//             {
//                 isSelecting = true;
//                 onObjectSelected.Invoke(currentTarget);
//             }
//         }
//         else
//         {
//             ResetSelection();
//         }
//     }

//     private void PerformRaycast(Vector3 rayOrigin, Vector3 rayDirection)
//     {
//         RaycastHit hit;
//         if (Physics.Raycast(rayOrigin, rayDirection, out hit, maxRayDistance, interactableLayer))
//         {
//             if (currentTarget != hit.collider.gameObject)
//             {
//                 currentTarget = hit.collider.gameObject;
//                 selectionTimer = 0f;
//                 onObjectHovered?.Invoke(currentTarget);
//             }
//             else
//             {
//                 selectionTimer += Time.deltaTime;
//                 if (selectionTimer >= selectionTime)
//                 {
//                     onObjectSelected.Invoke(currentTarget);
//                     selectionTimer = 0f;
//                 }
//             }
//         }
//         else
//         {
//             currentTarget = null;
//             onSelectionCancelled.Invoke();
//         }
//     }

//     private void UpdatePointerVisuals(Vector3 rayOrigin, Vector3 rayDirection)
//     {
//         if (pointerLine != null)
//         {
//             pointerLine.enabled = true;
//             pointerLine.SetPosition(0, rayOrigin);
//             pointerLine.SetPosition(1, rayOrigin + rayDirection * maxRayDistance);
//         }
//     }

//     private void ResetSelection()
//     {
//         if (currentTarget != null)
//         {
//             onSelectionCancelled.Invoke();
//         }
//         currentTarget = null;
//         selectionTimer = 0f;
//         isSelecting = false;
//     }
// }