using System.Collections;
using UnityEngine;

public class BoardingSpaceship : MonoBehaviour
{
    [SerializeField] DimmerUI dimmerUI;
    public Transform xrOrigin;
    public Transform shipSeatAnchor;
    public Transform shipExitAnchor;
    public Transform spaceship;

    
    [SerializeField] private bool isInShip = false;
    [SerializeField]  private bool isTeleporting = false;

    public void TeleportWithFade()
    {
        isTeleporting = true;

        //yield return StartCoroutine(fadeCanvas.FadeOut());
        dimmerUI.SetDuration(0.5f);
        dimmerUI.TransparentToOpaque();

        if (!isInShip)
        {
            xrOrigin.SetParent(spaceship);
            xrOrigin.position = shipSeatAnchor.position;
            xrOrigin.rotation = shipSeatAnchor.rotation;
            isInShip = true;
            Debug.Log("Player boarded the ship.");
        }
        else
        {
            xrOrigin.SetParent(null);
            xrOrigin.position = shipExitAnchor.position;
            xrOrigin.rotation = shipExitAnchor.rotation;
            isInShip = false;
            Debug.Log("Player exited the ship.");
        }

        // Fade in
        //yield return StartCoroutine(fadeCanvas.FadeIn());
        dimmerUI.OpaqueToTransparent();
        dimmerUI.SetDuration(5f);

        isTeleporting = false;
    }

    public void StartTeleport()
    {
        //StartCoroutine(TeleportWithFade());
        TeleportWithFade();
    }
}
