using UnityEngine;

public class GranuleController : MonoBehaviour
{
    [SerializeField]
    private GranuleModel model;

    [SerializeField]
    private GranuleView view;

    public void SetGranuleModel(GranuleModel newModel)
    {
        model = newModel;
        view.SetView(model);
    }

    public void SetGranuleVisibility(float visibility)
    {
        model.GranuleVisibility = visibility;
        view.SetView(model);
    }
}