using UnityEngine;

public class CopyCameraMove : MonoBehaviour
{
    public Transform playerCamera;
    public Transform anchorA;
    public Transform anchorB;

    void LateUpdate()
    {
        if (playerCamera == null || anchorA == null || anchorB == null) return;

        Vector3 localPos = anchorA.InverseTransformPoint(playerCamera.position);
        Vector3 newWorldPos = anchorB.TransformPoint(localPos);
        transform.position = newWorldPos;

        Quaternion localRot = Quaternion.Inverse(anchorA.rotation) * playerCamera.rotation;
        Quaternion newWorldRot = anchorB.rotation * localRot;
        transform.rotation = newWorldRot;
    }
}
