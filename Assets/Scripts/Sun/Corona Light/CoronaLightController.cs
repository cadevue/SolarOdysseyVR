using UnityEngine;
using UnityEngine.VFX;

public class CoronaLightController : MonoBehaviour 
{
    [SerializeField]
    private VisualEffect _visualEffect;

    public void SetCoronaLightActive(bool isActive)
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
        _visualEffect.gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        _visualEffect.gameObject.SetActive(false);
    }
}