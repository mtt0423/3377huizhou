using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GradientTextureGenerator : MonoBehaviour
{
    [Header("渐变设置")]
    public Gradient colorGradient;
    public int textureWidth = 256;
    public int textureHeight = 256;
    public string textureName = "ParticleGradient";

    [Header("形状设置")]
    public bool circularGradient = true; // 圆形渐变还是线性渐变
    public float softness = 0.3f; // 边缘柔化程度

    [ContextMenu("生成渐变纹理")]
    public void GenerateGradientTexture()
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                Color pixelColor;

                if (circularGradient)
                {
                    // 计算到中心的距离（0到1）
                    float centerX = textureWidth * 0.5f;
                    float centerY = textureHeight * 0.5f;
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                    float maxDistance = Mathf.Min(centerX, centerY);
                    float normalizedDistance = Mathf.Clamp01(distance / maxDistance);

                    // 应用柔化
                    float alpha = 1f - Mathf.Pow(normalizedDistance, 1f / softness);
                    Color gradientColor = colorGradient.Evaluate(normalizedDistance);
                    pixelColor = new Color(gradientColor.r, gradientColor.g, gradientColor.b, alpha);
                }
                else
                {
                    // 线性渐变
                    float normalizedX = x / (float)textureWidth;
                    Color gradientColor = colorGradient.Evaluate(normalizedX);
                    pixelColor = gradientColor;
                }

                texture.SetPixel(x, y, pixelColor);
            }
        }

        texture.Apply();

        // 保存纹理
        byte[] bytes = texture.EncodeToPNG();
        string path = "Assets/" + textureName + ".png";
        System.IO.File.WriteAllBytes(path, bytes);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
        Debug.Log("渐变纹理已生成: " + path);
#endif

        // 清理
        DestroyImmediate(texture);
    }

    // 为水波纹创建预设渐变
    [ContextMenu("创建水波纹预设")]
    public void CreateWaterRipplePreset()
    {
        // 设置水波纹的颜色渐变
        colorGradient = new Gradient();

        // 水波纹渐变：中心白色，边缘蓝色半透明
        GradientColorKey[] colorKeys = new GradientColorKey[3];
        colorKeys[0] = new GradientColorKey(Color.white, 0f);        // 中心：白色
        colorKeys[1] = new GradientColorKey(new Color(0.7f, 0.8f, 1f), 0.3f); // 中间：淡蓝色
        colorKeys[2] = new GradientColorKey(new Color(0.4f, 0.6f, 0.8f), 1f); // 边缘：蓝色

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[3];
        alphaKeys[0] = new GradientAlphaKey(0.8f, 0f);    // 中心：较不透明
        alphaKeys[1] = new GradientAlphaKey(0.5f, 0.5f);  // 中间：半透明
        alphaKeys[2] = new GradientAlphaKey(0f, 1f);      // 边缘：完全透明

        colorGradient.SetKeys(colorKeys, alphaKeys);

        circularGradient = true;
        softness = 0.2f;
        textureName = "WaterRippleGradient";

        Debug.Log("水波纹预设已配置，点击生成渐变纹理");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GradientTextureGenerator))]
public class GradientTextureGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GradientTextureGenerator generator = (GradientTextureGenerator)target;

        GUILayout.Space(10);

        if (GUILayout.Button("创建水波纹预设"))
        {
            generator.CreateWaterRipplePreset();
        }

        if (GUILayout.Button("生成渐变纹理"))
        {
            generator.GenerateGradientTexture();
        }
    }
}
#endif