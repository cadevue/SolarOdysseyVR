using UnityEngine;

public class ObjectPointerLineUI : MonoBehaviour
{
    [SerializeField] Transform pointedObject;
    [SerializeField] Transform vrPlayer;
    [SerializeField] GameObject arrow;

    private RectTransform guideUI;
    private LineRenderer uiPointerLine;

    private void Start()
    {
        guideUI = GetComponent<RectTransform>();
        uiPointerLine = GetComponent<LineRenderer>();

        SpawnArrow();
    }

    private void Update()
    {
        if (CheckIfObjectCovered() || CheckIfObjectOutOfBounds())
        {
            uiPointerLine.enabled = false;
            arrow.SetActive(false);
            return;
        }
        
        uiPointerLine.enabled = true;
        arrow.SetActive(true);
        SetPointerLine();
        SetArrow();
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

    private bool CheckIfObjectOutOfBounds()
    {
        Vector3 toTarget = (pointedObject.position - vrPlayer.position).normalized;
        Vector3 toGuideUI = (guideUI.position - vrPlayer.position).normalized;

        float angleBetween = Vector3.Angle(toTarget, toGuideUI);

        return angleBetween > 90f;
    }

    private void SpawnArrow()
    {
        if (arrow != null)
        {
            arrow = Instantiate(arrow, pointedObject.position, Quaternion.identity);
            arrow.transform.localScale = Vector3.one * 2f;
            arrow.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
        }
    }

    private void SetArrow()
    {
        if (arrow != null)
        {
            Vector3 arrowDirection = uiPointerLine.GetPosition(3) - uiPointerLine.GetPosition(2);

            if (arrowDirection.x < 5f)
            {
                arrow.transform.localScale = new Vector3(2f, arrowDirection.x * 10f, 2f);
            }

            Quaternion arrowRotation = Quaternion.FromToRotation(new Vector3(0, 1, 0), arrowDirection.normalized);
            arrow.transform.SetPositionAndRotation(uiPointerLine.GetPosition(3), arrowRotation);
        }
    }
}
