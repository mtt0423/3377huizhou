using UnityEngine;
using System.IO;

public class PaperTextureGenerator : MonoBehaviour
{
    [Header("纹理设置")]
    public int textureSize = 256;
    public string textureName = "PaperTexture";

    [Header("宣纸属性")]
    [Range(3f, 8f)]
    public float noiseScale = 5.0f;
    [Range(1.5f, 3f)]
    public float paperContrast = 2.0f;
    public Color paperColor1 = new Color(0.95f, 0.92f, 0.85f);
    public Color paperColor2 = new Color(0.85f, 0.80f, 0.70f);

    // 在Inspector中显示按钮
    [ContextMenu("生成宣纸纹理")]
    public void GeneratePaperTexture()
    {
        Texture2D paperTexture = new Texture2D(textureSize, textureSize);

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                // 多层噪声
                float noise1 = Mathf.PerlinNoise(x / (float)textureSize * noiseScale,
                                               y / (float)textureSize * noiseScale);
                float noise2 = Mathf.PerlinNoise((x + 100) / (float)textureSize * noiseScale * 2,
                                               (y + 100) / (float)textureSize * noiseScale * 2);

                float paperNoise = (noise1 * 0.7f + noise2 * 0.3f);
                paperNoise = Mathf.Pow(paperNoise, paperContrast);

                Color pixelColor = Color.Lerp(paperColor1, paperColor2, paperNoise);
                paperTexture.SetPixel(x, y, pixelColor);
            }
        }

        paperTexture.Apply();

        // 保存纹理
        byte[] bytes = paperTexture.EncodeToPNG();
        string path = Application.dataPath + "/" + textureName + ".png";
        File.WriteAllBytes(path, bytes);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log("宣纸纹理已生成: " + path);
#endif

        DestroyImmediate(paperTexture);
    }
}