using UnityEngine;

public class CopyCameraMove : MonoBehaviour
{
    public Transform playerCamera;
    public Transform anchorA;
    public Transform anchorB;

    void LateUpdate()
    {
        if (playerCamera == null || anchorA == null || anchorB == null) return;

        float distA = Vector3.Distance(anchorA.position, playerCamera.position);
        float distB = Vector3.Distance(anchorB.position, transform.position);

        float scaleFactor = distA / distB;

        anchorA.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        var m = anchorB.localToWorldMatrix * anchorA.worldToLocalMatrix * playerCamera.localToWorldMatrix;
        transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

        //Vector3 localPos = anchorA.InverseTransformPoint(playerCamera.position);
        //Vector3 newWorldPos = anchorB.TransformPoint(localPos);
        //transform.position = newWorldPos;

        //Quaternion localRot = Quaternion.Inverse(anchorA.rotation) * playerCamera.rotation;
        //Quaternion newWorldRot = anchorB.rotation * localRot;
        //transform.rotation = newWorldRot;
    }
}
