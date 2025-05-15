using UnityEngine;

//TEMP, created by chatGPT
public class TileableNoiseGenerator : MonoBehaviour
{
    
    public int textureWidth = 256;
    public int textureHeight = 256;

    [Header("Noise Settings")]
    [Tooltip("Determines how many noise 'features' tile across the texture. Higher values mean more, smaller features.")]
    public float noiseScale = 5.0f;
    [Tooltip("Offsets the noise pattern in the X direction.")]
    public float offsetX = 0.0f;
    [Tooltip("Offsets the noise pattern in the Y direction.")]
    public float offsetY = 0.0f;

    [Header("Output")]
    [Tooltip("The Renderer to apply the generated texture to. If null, tries to get Renderer on this GameObject.")]
    public Renderer targetRenderer;
    [Tooltip("If true, the texture will be generated and applied in Start().")]
    public bool generateOnStart = true;

    private Texture2D generatedTexture;

    void Start()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        if (generateOnStart)
        {
            GenerateAndApplyTexture();
        }
    }

    [ContextMenu("Generate Tileable Noise Texture")]
    public void GenerateAndApplyTexture()
    {
        if (textureWidth <= 0 || textureHeight <= 0)
        {
            Debug.LogError("Texture dimensions must be greater than 0.");
            return;
        }
        if (noiseScale <= 0)
        {
            Debug.LogWarning("Noise scale should ideally be greater than 0 for meaningful results.");
            // Allow it, but it might produce flat noise.
        }

        if (generatedTexture != null)
        {
            // Clean up the old texture if we're regenerating
            if (Application.isPlaying)
            {
                Destroy(generatedTexture);
            }
            else
            {
                DestroyImmediate(generatedTexture);
            }
        }

        generatedTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, false);
        generatedTexture.name = "TileableNoiseTexture";
        generatedTexture.wrapMode = TextureWrapMode.Repeat; // Good for tiling

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                // Normalized coordinates for current pixel (0 to 1)
                float u = (float)x / textureWidth;
                float v = (float)y / textureHeight;

                // Calculate noise coordinates.
                // 'noiseScale' determines the period of the noise in the texture.
                // E.g., if noiseScale is 5, the pattern will repeat 5 times across the texture.
                float px_0 = u * noiseScale + offsetX;
                float py_0 = v * noiseScale + offsetY;
                float px_1 = (u - 1.0f) * noiseScale + offsetX; // Corresponds to px_0 - noiseScale
                float py_1 = (v - 1.0f) * noiseScale + offsetY; // Corresponds to py_0 - noiseScale

                // Sample Perlin noise at 4 points to ensure tiling
                // p00 is the "main" sample
                // p10 samples from the "left" (x-direction wrapped)
                // p01 samples from the "bottom" (y-direction wrapped)
                // p11 samples from the "bottom-left" (both x and y wrapped)
                float p00 = Mathf.PerlinNoise(px_0, py_0);
                float p10 = Mathf.PerlinNoise(px_1, py_0);
                float p01 = Mathf.PerlinNoise(px_0, py_1);
                float p11 = Mathf.PerlinNoise(px_1, py_1);

                // Bilinear interpolation of the four noise samples
                // The blend factors are u and v themselves.
                // Interpolation: (1-t)*a + t*b
                // Horizontal interp for top row: (1-u)*p00 + u*p10
                // Horizontal interp for bottom row: (1-u)*p01 + u*p11
                // Vertical interp between these two results: (1-v)*h_top + v*h_bottom
                float noiseValue = (1 - u) * (1 - v) * p00 + // Contribution from p00, weighted by distance to other corners
                                   u * (1 - v) * p10 +       // Contribution from p10
                                   (1 - u) * v * p01 +       // Contribution from p01
                                   u * v * p11;             // Contribution from p11

                generatedTexture.SetPixel(x, y, new Color(noiseValue, noiseValue, noiseValue));
            }
        }

        generatedTexture.Apply();

        if (targetRenderer != null && targetRenderer.sharedMaterial != null)
        {
            targetRenderer.sharedMaterial.mainTexture = generatedTexture;
        }
        else if (targetRenderer != null && targetRenderer.material != null) // Fallback for instances
        {
             targetRenderer.material.mainTexture = generatedTexture;
        }
        else
        {
            Debug.LogWarning("Target Renderer or its material is not set. Texture generated but not applied.");
        }
    }

    void OnDestroy()
    {
        // Clean up texture when the script/object is destroyed
        if (generatedTexture != null)
        {
            if (Application.isPlaying)
            {
                Destroy(generatedTexture);
            }
            else
            {
                DestroyImmediate(generatedTexture);
            }
        }
    }
}