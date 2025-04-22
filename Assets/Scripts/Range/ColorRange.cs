using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Range
{
    public class HSVColorRange
    {
        public ReactiveRange<float> HueRange { get; private set; }
        public ReactiveRange<float> SaturationRange { get; private set; }
        public ReactiveRange<float> ValueRange { get; private set; }
        public ReactiveRange<float> OpacityRange { get; private set; }

        public HSVColorRange(ReactiveRange<float> hueRange, ReactiveRange<float> saturationRange, ReactiveRange<float> valueRange, ReactiveRange<float> opacityRange)
        {
            HueRange = hueRange;
            SaturationRange = saturationRange;
            ValueRange = valueRange;
            OpacityRange = opacityRange;
        }

        public HSVColorRange() : this(new ReactiveRange<float>(0.0f, 1.0f, 0.0f, 1.0f),
                                    new ReactiveRange<float>(0.0f, 1.0f, 0.0f, 1.0f),
                                    new ReactiveRange<float>(0.0f, 1.0f, 0.0f, 1.0f),
                                    new ReactiveRange<float>(0.0f, 1.0f, 0.0f, 1.0f)) { }

        public float4 GetRandomColor(ref Random random)
        {
            float4 randomColor = new float4(HueRange.GetRandomValue(ref random),
                                            SaturationRange.GetRandomValue(ref random),
                                            ValueRange.GetRandomValue(ref random),
                                            OpacityRange.GetRandomValue(ref random));
            return randomColor;
        }

        public float4 GetRandomRGBColor(ref Random random)
        {
            float4 randomHSVColor = GetRandomColor(ref random);
            Color randomRGBColor = Color.HSVToRGB(randomHSVColor.x, randomHSVColor.y, randomHSVColor.z);
            float4 randomColor = new float4(randomRGBColor.r, randomRGBColor.g, randomRGBColor.b, randomHSVColor.w);
            return randomColor;
        }
    }
}
