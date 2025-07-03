using UnityEngine;
using NaughtyAttributes;

public class CoronalLoopManager : MonoBehaviour
{
    [SerializeField] private CoronalLoop[] _coronalLoops;

    [Button]
    public void AssignCoronalLoopsFromChildren()
    {
        _coronalLoops = GetComponentsInChildren<CoronalLoop>();
        Debug.Log($"Assigned {_coronalLoops.Length} coronal loops.");
    }

    public void SetCoronalLoopsActive(bool isActive)
    {
        if (isActive)
        {
            EnableCoronalLoops();
        }
        else
        {
            DisableCoronalLoops();
        }
    }

    private void EnableCoronalLoops()
    {
        foreach (var loop in _coronalLoops)
        {
            loop.EnableCoronalLoop();
        }
    }

    private void DisableCoronalLoops()
    {
        foreach (var loop in _coronalLoops)
        {
            loop.DisableCoronalLoop();
        }
    }
}