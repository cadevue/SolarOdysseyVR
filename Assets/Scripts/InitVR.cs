using UnityEngine;

public class InitVR : MonoBehaviour
{
    private void Awake()
    {
        // Set target frame rate to 90 FPS for VR
        Application.targetFrameRate = 90;
        // OVRManager.display.displayRefreshRate = 90f; // Set the refresh rate for Oculus devices

        // Foveated rendering settings
        // OVRManager.foveatedRenderingLevel = OVRManager.FoveatedRenderingLevel.High; // Set foveated rendering level
    }
}
