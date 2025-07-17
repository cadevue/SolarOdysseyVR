using UnityEngine;

public class SunspotView : MonoBehaviour
{
    [SerializeField] private Material _sunspotMaterial;
    private ComputeBuffer _sunspotBuffer;

    public void Init()
    {
        _sunspotBuffer = new ComputeBuffer(0, sizeof(float) * 4 + sizeof(int));
    }

    public void UpdateSunspotView(SunspotBufferData[] sunspots)
    {
        _sunspotBuffer.SetData(sunspots);
        _sunspotMaterial.SetBuffer("_SunspotBuffer", _sunspotBuffer);
    }

    private void OnDestroy()
    {
        _sunspotBuffer?.Release();
        _sunspotBuffer = null;
    }
}