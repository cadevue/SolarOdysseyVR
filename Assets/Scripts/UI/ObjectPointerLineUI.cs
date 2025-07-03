using UnityEngine;

public class ObjectPointerLineUI : MonoBehaviour
{
    [SerializeField] Transform pointedObject;
    [SerializeField] Transform vrPlayer;

    private RectTransform guideUI;
    private LineRenderer uiPointerLine;
    private bool isObjectCovered = false;

    private void Start()
    {
        guideUI = GetComponent<RectTransform>();
        uiPointerLine = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        isObjectCovered = CheckIfObjectCovered();

        if (isObjectCovered)
        {
            uiPointerLine.enabled = false;
            return;
        }
        
        uiPointerLine.enabled = true;
        SetPointerLine();
    }

    private void SetPointerLine()
    {
        Vector3 toTarget = pointedObject.position - vrPlayer.position;

        float guideUIDistance = (guideUI.position - vrPlayer.position).magnitude;

        Vector3 projectedObjectPoint = vrPlayer.position + Vector3.ClampMagnitude(toTarget, guideUIDistance);

        int onRight = projectedObjectPoint.x > guideUI.position.x ? 1 : -1;

        Vector3 attachPointLocalOffset = new Vector3(guideUI.rect.width * 0.5f * onRight, 0f, 0f);
        Vector3 attachPointWorldOffset = guideUI.TransformPoint(attachPointLocalOffset);

        float xDistanceToProjectedPoint = Mathf.Abs(projectedObjectPoint.x - attachPointWorldOffset.x) * onRight;

        uiPointerLine.SetPosition(0, attachPointWorldOffset);
        uiPointerLine.SetPosition(1, attachPointWorldOffset + new Vector3(xDistanceToProjectedPoint * 0.25f, 0f, 0f));
        uiPointerLine.SetPosition(2, projectedObjectPoint - new Vector3(xDistanceToProjectedPoint * 0.25f, 0f, 0f));
        uiPointerLine.SetPosition(3, projectedObjectPoint);

        uiPointerLine.startWidth = 0.03f;
        uiPointerLine.endWidth = 0.03f;
    }

    private bool CheckIfObjectCovered()
    {
        Vector3 objectOnScreenPos = Camera.main.WorldToScreenPoint(pointedObject.position);

        bool isCovered = RectTransformUtility.RectangleContainsScreenPoint(guideUI, objectOnScreenPos, Camera.main);

        return isCovered;
    }
}
