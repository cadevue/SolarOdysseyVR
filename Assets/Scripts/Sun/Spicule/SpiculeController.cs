using UnityEngine;
using UnityEngine.VFX;

public class SpiculeController : MonoBehaviour
{
    [SerializeField]
    private VisualEffect _visualEffect;

    public void SetSpiculeActive(bool isActive)
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