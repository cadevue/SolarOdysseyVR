using UnityEngine;

public class SolarCycleController : MonoBehaviour
{
    [SerializeField]
    private SolarCycleModel model;

    public void SetCycleProgress(float progress)
    {
        model.CycleProgress = progress;
    }
}