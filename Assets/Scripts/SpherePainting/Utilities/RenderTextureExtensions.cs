using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace SpherePainting
{
    public static class RenderTextureExtensions
    {
        // テクスチャをPNG形式でエクスポート
        public static void ExportAsPNG(this RenderTexture renderTexture, string folderPath, string fileName)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var texture = new Texture2D(renderTexture.width, renderTexture.height, GraphicsFormatUtility.GetTextureFormat(renderTexture.graphicsFormat), false);
            var tmp = RenderTexture.active;
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = tmp;
            string filePath = Path.Combine(folderPath, $"{fileName}.png");
            File.WriteAllBytes(filePath, texture.EncodeToPNG());
            Object.Destroy(texture);
        }

        // テクスチャを透明にする。
        public static void ClearOut(this RenderTexture renderTexture)
        {
            RenderTexture rt = RenderTexture.active;
            RenderTexture.active = renderTexture;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = rt;
        }
    }
}