using UnityEngine;
using Random = Unity.Mathematics.Random;
using System.Runtime.InteropServices;
using System;
using Unity.Mathematics;
using Range;
using WeightedValues;

namespace SpherePainting
{
    public enum BlendMode
    { 
        NORMAL,     // 通常
        MULTIPLY,   // 乗算
        OVERLAY,    // オーバーレイ
        ADD,        // 加算
        ADD_GLOW,   // 加算（発光）
        DIFFERENCE, // 差分
        EXCLUSION,  // 排他
        HUE,        // 色相
        SATURATION, // 彩度
        COLOR,      // 色
        LUMINOSITY  // 明度
    }

    [Serializable]
    public struct SphereMaterialData
    {
        [field:SerializeField] public float4 Color { get; set; }
        [field:SerializeField] public BlendMode BlendMode { get; set; }

        public static int GetSize()
        {
            return Marshal.SizeOf<SphereMaterialData>();
        }

        // マテリアルデータを補間
        public static SphereMaterialData Lerp(SphereMaterialData a, SphereMaterialData b, float t)
        {
            SphereMaterialData data = new ()
            {
                Color = new float4(Mathf.Lerp(a.Color.x, b.Color.x, t),
                                   Mathf.Lerp(a.Color.y, b.Color.y, t),
                                   Mathf.Lerp(a.Color.z, b.Color.z, t),
                                   Mathf.Lerp(a.Color.w, b.Color.w, t)),
                BlendMode = t < 0.5 ? a.BlendMode : b.BlendMode
            };
            return data;
        }

        // ランダムなマテリアルデータを生成
        public static SphereMaterialData GenerateRandom(HSVColorRange colorRange, IWeightedValues<BlendMode> weightedBlendModes, ref Random random)
        {
            SphereMaterialData data = new ()
            {
                Color = colorRange.GetRandomRGBColor(ref random),
                BlendMode = weightedBlendModes.GetRandomValue(ref random)
            };
            return data;
        }

        // ベースデータを元にマテリアルの変異を作成
        public static SphereMaterialData GenerateVariant(SphereMaterialData baseData, HSVColorRange colorRange, IWeightedValues<BlendMode> weightedBlendModes, float randomness, ref Random random)
        {
            SphereMaterialData data = Lerp(baseData, GenerateRandom(colorRange, weightedBlendModes, ref random), randomness);
            return data;
        }
    };
}
