using UnityEngine;
using NaughtyAttributes;

public class SunBaseController : MonoBehaviour
{
    [SerializeField]
    private SunBaseModel model;

    [SerializeField]
    private SunBaseView view;

    public void SetSunBaseModel(SunBaseModel newModel)
    {
        model = newModel;
        RefreshView();
    }

    [Button]
    public void RefreshView()
    {
        view.SetView(model);
    }
}