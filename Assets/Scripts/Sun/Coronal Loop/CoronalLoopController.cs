using UnityEngine;

public class CoronalLoopController : MonoBehaviour
{
    [SerializeField]
    private CoronalLoopModel model;

    [SerializeField]
    private CoronalLoopView view;

    public void SetCoronalLoopModel(CoronalLoopModel newModel)
    {
        model = newModel;
        view.SetView(model);
    }
}