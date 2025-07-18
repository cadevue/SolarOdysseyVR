using UnityEngine;
using UnityEngine.VFX;

public class CoronalLoopController : MonoBehaviour
{

    [SerializeField]
    private VisualEffect[] _visualEffects;

    public void SetCoronalLoopActive(bool isActive)
    {
        if (isActive)
        {
            TurnOn();
        }
        else
        {
            TurnOff();
        }
    }

    public void TurnOn()
    {
        foreach (var vfx in _visualEffects)
        {
            vfx.gameObject.SetActive(true);
        }
    }

    public void TurnOff()
    {
        foreach (var vfx in _visualEffects)
        {
            vfx.gameObject.SetActive(false);
        }
    }

    // [SerializeField]
    // private CoronalLoopModel model;

    // [SerializeField]
    // private CoronalLoopView view;

    // public void SetCoronalLoopModel(CoronalLoopModel newModel)
    // {
    //     model = newModel;
    //     view.SetView(model);
    // }
}