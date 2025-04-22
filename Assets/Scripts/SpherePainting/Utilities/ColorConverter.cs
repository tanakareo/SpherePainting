using Unity.Mathematics;
using UnityEngine;

namespace SpherePainting
{
    public static class ColorConverter
    {
        public static float3 HSVToRGBColor(this float3 hsvColor)
        {
            Color rgbColor = Color.HSVToRGB(hsvColor.x, hsvColor.y, hsvColor.z);
            return new float3(rgbColor.r, rgbColor.g, rgbColor.b);
        }

        public static float4 HSVToRGBColor(this float4 hsvColor)
        {
            return new float4(hsvColor.xyz.HSVToRGBColor(), hsvColor.w);
        }

        public static Color ToColor(this float3 color)
        {
            return new Color(color.x, color.y, color.z);
        }

        public static string HSVtoHex(float3 hsv)
        {
            // HSV → RGB 変換
            Color rgbColor = Color.HSVToRGB(hsv.x, hsv.y, hsv.z);
            
            // 0-255 の範囲に変換
            int r = Mathf.RoundToInt(rgbColor.r * 255);
            int g = Mathf.RoundToInt(rgbColor.g * 255);
            int b = Mathf.RoundToInt(rgbColor.b * 255);

            // 16進数カラーコードを生成
            return $"#{r:X2}{g:X2}{b:X2}";
        }
    }
}