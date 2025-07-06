using NaughtyAttributes;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class CameraSnapshotRender : MonoBehaviour
{
    [SerializeField] private Camera _targetCamera;
    [SerializeField] private int _pixelWidth = 1920;
    [SerializeField] private int _pixelHeight = 1080;
    [SerializeField] private string _snapshotName = "SunRender_01";
    [SerializeField] private LayerMask _renderMask = -1; // Default to all layers


    [Button]
    public void TakeBasicScreenshot()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Screenshots can only be taken in Play mode.");
            return;
        }

        string path = Application.dataPath + "/Renders/";
        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);

        string screenshotFileName = _snapshotName + ".png";
        string fullPath = path + screenshotFileName;

        // Check if there's an existing file in that path
        if (System.IO.File.Exists(fullPath))
        {
            Debug.LogWarning("File already exists at " + fullPath + ". It will be overwritten.");
            return;
        }

        // Simple screenshot capture
        ScreenCapture.CaptureScreenshot(fullPath, 1);
    }


    [Button]
    public void TakeSnapshot()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Snapshots can only be taken in Play mode.");
            return;
        }

        string path = Application.dataPath + "/Renders/";
        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);

        string snapshotFileName = _snapshotName + ".png";
        string fullPath = path + snapshotFileName;
        StartCoroutine(TakeTransparentSnapshot(_targetCamera, _pixelWidth, _pixelHeight, fullPath));
    }

    public static IEnumerator TakeTransparentSnapshot(Camera cam, int width, int height, string path)
    {
        // Store original camera state
        var originalClearFlags = cam.clearFlags;
        var originalBG = cam.backgroundColor;

        // --- Step 1: Render with post-processing (opaque)
        RenderTexture colorRT = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = colorRT;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black; // background won't matter

        cam.Render(); // post-processing is on

        yield return new WaitForEndOfFrame();

        Texture2D colorTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        RenderTexture.active = colorRT;
        colorTex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        colorTex.Apply();

        // --- Step 2: Render with transparent background (no post-processing)
        RenderTexture alphaRT = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        cam.GetUniversalAdditionalCameraData().renderPostProcessing = false; // Disable post-processing
        cam.targetTexture = alphaRT;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0); // transparent
        cam.Render(); // render without post-processing

        yield return new WaitForEndOfFrame();

        Texture2D alphaTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        RenderTexture.active = alphaRT;
        alphaTex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        alphaTex.Apply();

        // --- Step 3: Merge alpha
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color color = colorTex.GetPixel(x, y);
                float alpha = alphaTex.GetPixel(x, y).a;
                color.a = alpha;
                colorTex.SetPixel(x, y, color);
            }
        }

        colorTex.Apply();

        // --- Step 4: Save as PNG
        byte[] png = colorTex.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, png);
        Debug.Log("Saved transparent snapshot: " + path);

        // --- Cleanup
        RenderTexture.active = null;
        cam.GetUniversalAdditionalCameraData().renderPostProcessing = true; // Re-enable post-processing
        cam.targetTexture = null;
        cam.clearFlags = originalClearFlags;
        cam.backgroundColor = originalBG;

        Destroy(colorRT);
        Destroy(alphaRT);
        Destroy(colorTex);
        Destroy(alphaTex);
    }
}
