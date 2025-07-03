using UnityEngine;

public class ObjectPointerLineUI : MonoBehaviour
{
    [SerializeField] Transform pointedObject;
    [SerializeField] Transform vrPlayer;

    private RectTransform guideUI;
    private LineRenderer uiPointerLine;

    private void Start()
    {
        guideUI = this.GetComponent<RectTransform>();
        uiPointerLine = this.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        SetPointerLine();
    }

    private void SetPointerLine()
    {
        Vector3 toTarget = pointedObject.position - vrPlayer.position;

        float guideUIDistance = (guideUI.position - vrPlayer.position).magnitude;

        Vector3 projectedObjectPoint = vrPlayer.position + Vector3.ClampMagnitude(toTarget, guideUIDistance);

        Vector3 attachPointLocalOffset = new Vector3(guideUI.rect.width * 0.5f, 0f, 0f);
        Vector3 attachPointWorldOffset = guideUI.TransformPoint(attachPointLocalOffset);

        float xDistanceToProjectedPoint = Mathf.Abs(projectedObjectPoint.x - attachPointWorldOffset.x);

        uiPointerLine.SetPosition(0, attachPointWorldOffset);
        uiPointerLine.SetPosition(1, attachPointWorldOffset + new Vector3(xDistanceToProjectedPoint * 0.25f, 0f, 0f));
        uiPointerLine.SetPosition(2, projectedObjectPoint - new Vector3(xDistanceToProjectedPoint * 0.25f, 0f, 0f));
        uiPointerLine.SetPosition(3, projectedObjectPoint);

        uiPointerLine.startWidth = 0.03f;
        uiPointerLine.endWidth = 0.03f;
    }
}
