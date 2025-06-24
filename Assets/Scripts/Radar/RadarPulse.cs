using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class RadarPulse : MonoBehaviour
{
    public float pulseSpeed = 2f;
    public float maxRadius = 20f;
    public float minRadius = 5f;

    private SphereCollider radarCollider;
    private float currentRadius;
    private bool expanding = true;
    private Material radarMaterial;

    void Start()
    {
        radarCollider = GetComponent<SphereCollider>();
        radarCollider.isTrigger = true;
        currentRadius = radarCollider.radius;

        // Get the material from the sphere mesh (optional)
        var rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            radarMaterial = rend.material;
    }

    void Update()
    {
        // Pulsing radius
        if (expanding)
        {
            currentRadius += Time.deltaTime * pulseSpeed;
            if (currentRadius >= maxRadius)
                expanding = false;
        }
        else
        {
            currentRadius -= Time.deltaTime * pulseSpeed;
            if (currentRadius <= minRadius)
                expanding = true;
        }

        radarCollider.radius = currentRadius;

        // Optional: scale the mesh for visual feedback
        transform.localScale = Vector3.one * currentRadius * 2f;

        // Optional: pulse transparency
        if (radarMaterial != null)
        {
            Color color = radarMaterial.color;
            color.a = Mathf.Lerp(0.1f, 0.3f, Mathf.InverseLerp(minRadius, maxRadius, currentRadius));
            radarMaterial.color = color;
        }
    }
}

