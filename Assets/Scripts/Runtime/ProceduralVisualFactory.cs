using System.Collections.Generic;
using UnityEngine;

public static class ProceduralVisualFactory
{
    private static readonly Dictionary<string, Material> MaterialCache = new Dictionary<string, Material>();

    public static Material GetMarbleMaterial(string key, Color baseColor, Color veinColor, float smoothness, float metallic)
    {
        if (MaterialCache.TryGetValue(key, out Material cached))
        {
            return cached;
        }

        Material material = new Material(Shader.Find("Standard"));
        material.name = key;
        material.mainTexture = CreateMarbleTexture(baseColor, veinColor);
        material.mainTextureScale = new Vector2(4f, 4f);
        material.color = Color.white;
        material.SetFloat("_Glossiness", smoothness);
        material.SetFloat("_Metallic", metallic);
        MaterialCache[key] = material;
        return material;
    }

    public static Material GetColorMaterial(string key, Color color, float smoothness, float metallic, Color? emission = null)
    {
        if (MaterialCache.TryGetValue(key, out Material cached))
        {
            return cached;
        }

        Material material = new Material(Shader.Find("Standard"));
        material.name = key;
        material.color = color;
        material.SetFloat("_Glossiness", smoothness);
        material.SetFloat("_Metallic", metallic);
        if (emission.HasValue)
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", emission.Value);
        }

        MaterialCache[key] = material;
        return material;
    }

    public static Material GetFaceMaterial(string key, Color skin, Color iris, Color brow, Color beard, Color accent, bool beardHeavy)
    {
        if (MaterialCache.TryGetValue(key, out Material cached))
        {
            return cached;
        }

        Texture2D texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        texture.name = key + "_Face";
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        Color clear = new Color(0f, 0f, 0f, 0f);
        Color[] pixels = new Color[64 * 64];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = clear;
        }

        FillRect(pixels, 64, 10, 8, 44, 48, skin);
        FillRect(pixels, 64, 16, 18, 10, 6, Color.white);
        FillRect(pixels, 64, 38, 18, 10, 6, Color.white);
        FillRect(pixels, 64, 20, 20, 4, 4, iris);
        FillRect(pixels, 64, 42, 20, 4, 4, iris);
        FillRect(pixels, 64, 14, 14, 14, 3, brow);
        FillRect(pixels, 64, 36, 14, 14, 3, brow);
        FillRect(pixels, 64, 29, 28, 6, 12, new Color(0.85f, 0.72f, 0.62f, 1f));
        FillRect(pixels, 64, 22, 42, 20, 3, accent);

        if (beardHeavy)
        {
            FillRect(pixels, 64, 16, 38, 32, 13, beard);
            FillRect(pixels, 64, 22, 34, 20, 8, beard);
        }
        else
        {
            FillRect(pixels, 64, 12, 24, 5, 18, accent);
            FillRect(pixels, 64, 47, 24, 5, 18, accent);
            FillRect(pixels, 64, 26, 6, 12, 5, accent);
        }

        texture.SetPixels(pixels);
        texture.Apply();

        Material material = new Material(Shader.Find("Unlit/Transparent"));
        material.name = key;
        material.mainTexture = texture;
        MaterialCache[key] = material;
        return material;
    }

    public static Material GetTransparentMaterial(string key, Color color)
    {
        if (MaterialCache.TryGetValue(key, out Material cached))
        {
            return cached;
        }

        Material material = new Material(Shader.Find("Unlit/Transparent"));
        material.name = key;
        material.color = color;
        MaterialCache[key] = material;
        return material;
    }

    private static Texture2D CreateMarbleTexture(Color baseColor, Color veinColor)
    {
        Texture2D texture = new Texture2D(256, 256, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Repeat;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float nx = x / (float)texture.width;
                float ny = y / (float)texture.height;
                float noiseA = Mathf.PerlinNoise(nx * 4.3f, ny * 4.1f);
                float noiseB = Mathf.PerlinNoise(nx * 13.1f + 8.7f, ny * 12.2f + 2.9f);
                float veins = Mathf.SmoothStep(0.55f, 0.88f, Mathf.Abs(Mathf.Sin((nx + ny * 0.35f + noiseB * 0.2f) * 19f)));
                Color color = Color.Lerp(baseColor * (0.92f + noiseA * 0.12f), veinColor, veins * 0.25f);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private static void FillRect(Color[] pixels, int width, int x, int y, int rectWidth, int rectHeight, Color color)
    {
        for (int yy = y; yy < y + rectHeight; yy++)
        {
            for (int xx = x; xx < x + rectWidth; xx++)
            {
                if (xx < 0 || yy < 0 || xx >= width || yy >= pixels.Length / width)
                {
                    continue;
                }

                pixels[yy * width + xx] = color;
            }
        }
    }
}
