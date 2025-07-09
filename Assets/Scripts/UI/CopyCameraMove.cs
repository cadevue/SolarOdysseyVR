using UnityEngine;

public class CopyCameraMove : MonoBehaviour
{
    [SerializeField] Transform playerCamera;
    [SerializeField] Transform anchorA;
    [SerializeField] Transform anchorB;

    private void Awake()
    {
        float distA = Vector3.Distance(anchorA.position, playerCamera.position);
        float distB = Vector3.Distance(anchorB.position, transform.position);

        float scaleFactor = distA / distB;
        anchorA.localScale = Vector3.one * scaleFactor;
    }

    void LateUpdate()
    {
        if (playerCamera == null || anchorA == null || anchorB == null) return;

        var m = anchorB.localToWorldMatrix * anchorA.worldToLocalMatrix * playerCamera.localToWorldMatrix;
        transform.SetPositionAndRotation(m.GetColumn(3), ExtractRotation(m));
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
