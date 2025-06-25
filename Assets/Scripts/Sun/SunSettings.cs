using UnityEngine;

public class SunSettings : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _coronalLoops;
    [SerializeField] private Transform _prominence;

    public void OnCoronalLoopsToggled(bool isOn)
    {
        _coronalLoops.gameObject.SetActive(isOn);
    }

    public void OnProminenceToggled(bool isOn)
    {
        _prominence.gameObject.SetActive(isOn);
    }
}
