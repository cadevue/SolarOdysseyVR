using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ObjectDetector : MonoBehaviour
{
    [System.Serializable]
    public class TagProperties
    {
        public string tag;
        [ColorUsage(true, true)]
        public Color color;
        public float scale = 1f;
    }

    public GameObject moveableObject;
    public GameObject spaceshipLocomotion;

    public Transform radarCenter;
    public RectTransform canvasCenter;
    public GameObject blipPrefab;
    public Material blipHoloMaterial;
    public Transform blipContainer;
    public List<TagProperties> tagProperties = new();

    public Material holoMaterial;

    private float radarRadius;
    private float canvasRadius;
    private float blipScale;

    private Dictionary<Transform, GameObject> activeBlips = new();

    public bool scanVisual = false;
    public GameObject scannerPrefab;
    private float scanDuration = 10f;
    private float scanTimer = 0f;
    public float scanInterval = 5f;
    public float scanSize;

    private void Start()
    {
        radarRadius = GetComponent<SphereCollider>().radius;
        scanSize = radarRadius * 2;
        canvasRadius = canvasCenter.rect.width / 2f;
        blipScale = blipPrefab.transform.localScale.y;

        spaceshipLocomotion = moveableObject.GetNamedChild("Locomotion");
    }

    void OnTriggerEnter(Collider other)
    {
        string objTag = other.tag;
        Transform target = other.transform;

        int tagIndex = IsTagInList(objTag);
        if (tagIndex == -1 || activeBlips.ContainsKey(target)) return;

        GameObject blip = Instantiate(blipPrefab, blipContainer);
        blip.transform.localScale *= tagProperties[tagIndex].scale;

        XRSimpleInteractable xRSimpleInteractable = blip.GetComponent<XRSimpleInteractable>();
        xRSimpleInteractable.selectEntered.AddListener((SelectEnterEventArgs args) =>
        {
            spaceshipLocomotion.GetComponent<SpaceshipLocomotion>().MoveToTarget(target.transform);
        });

        SetBlip(target, blip);
        SetBlipMaterial(target, blip);

        activeBlips[target] = blip;
    }

    void OnTriggerExit(Collider other)
    {
        Transform target = other.transform;
        if (activeBlips.TryGetValue(target, out GameObject blip))
        {
            Destroy(blip);
            activeBlips.Remove(target);
        }
    }

    void Update()
    {
        foreach (var pair in activeBlips)
        {
            Transform target = pair.Key;
            GameObject blip = pair.Value;

            SetBlip(target, blip);
        }

        if (scanVisual)
        {
            scanTimer -= Time.deltaTime;
            if (scanTimer <= 0f)
            {
                SpawnScannerParticle();
                scanTimer = scanInterval;
            }
        }
    }

    private int IsTagInList(string tag)
    {
        for (int i = 0; i < tagProperties.Count; i++)
        {
            if (tagProperties[i].tag.Equals(tag))
                return i;
        }
        return -1;
    }

    private void SetBlip(Transform target, GameObject blip) 
    {
        Vector3 relative = target.position - radarCenter.position;
        float scale = canvasRadius / radarRadius;
        Vector3 scaledPosition = relative * scale;

        blip.transform.localPosition = scaledPosition;

        LineRenderer lineRenderer = blip.GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = 0.001f;
            lineRenderer.endWidth = 0.001f;

            float elevation = target.position.y - radarCenter.position.y;

            Vector3 start = Vector3.zero;
            Vector3 end = new Vector3(0f, -elevation / blipScale, 0f);

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }
    }

    private void SetBlipMaterial(Transform target, GameObject blip)
    {
        Renderer renderer = blip.GetComponent<Renderer>();

        string objTag = target.tag;
        int tagIndex = IsTagInList(objTag);

        if (tagIndex == -1) return;

        Color color = tagProperties[tagIndex].color;

        if (objTag == "Planet")
        {
            GameObject planetObject = target.transform.GetChild(1).gameObject;
            Material planetMaterial = planetObject.GetComponent<Renderer>().material;

            renderer.materials = new Material[] { planetMaterial, holoMaterial};
        }
        else
        {
            renderer.material = holoMaterial;
        }

        LineRenderer lineRenderer = blip.GetComponent<LineRenderer>();
        if (lineRenderer != null)
            lineRenderer.material.color = color;

    }

    private void SpawnScannerParticle()
    {
        GameObject scanner = Instantiate(scannerPrefab, gameObject.transform.position, Quaternion.identity);
        ParticleSystem scannerParticle = scanner.transform.GetChild(0).GetComponent<ParticleSystem>();

        if (scannerParticle != null)
        {
            var scannerMain = scannerParticle.main;
            scannerMain.startLifetime = scanDuration;
            scannerMain.startSize = scanSize;
        }

        Destroy(scanner, scanDuration + 1);
    }
}
