using UnityEngine;

public class SunBaseController : MonoBehaviour
{
    [SerializeField]
    private SunBaseModel model;

    [SerializeField]
    private SunBaseView view;

    public void SetSunBaseModel(SunBaseModel newModel)
    {
        model = newModel;
        view.SetView(model);
    }
}