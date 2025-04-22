using System;
using Newtonsoft.Json;
using UnityEngine;
using Range;
using WeightedValues;
using System.Linq;

namespace SpherePainting
{
    public static class SphereMaterialDataCreatorDataHandler
    {
        [Serializable]
        public class SphereMaterialDataCreatorData
        {
            [JsonProperty] public uint Seed { get; private set; }
            [JsonProperty] public Vector2 OperationAreaMaterialRandomnessRange { get; private set; }
            [JsonProperty] public Vector2 OperationTargetMaterialRandomnessRange { get; private set; }
            [JsonProperty] public Vector2 MaterialHueRange { get; private set; }
            [JsonProperty] public Vector2 MaterialSaturationRange { get; private set; }
            [JsonProperty] public Vector2 MaterialValueRange { get; private set; }
            [JsonProperty] public Vector2 MaterialOpacityRange { get; private set; }
            [JsonProperty] public Vector2 OperationTargetMaterialOpacityRange { get; private set; }
            [JsonProperty] public float TransparentOperationTargetRatio { get; private set; }
            [JsonProperty] public WeightedItem<BlendMode>[] WeightedBlendModes { get; private set; }
            
            public SphereMaterialDataCreatorData(){}

            public SphereMaterialDataCreatorData(SphereMaterialDataCreator sphereMaterialDataCreator)
            {
                Seed = sphereMaterialDataCreator.Seed.CurrentValue;
                OperationAreaMaterialRandomnessRange = sphereMaterialDataCreator.OperationAreaMaterialRandomnessRange.ToVector2();
                OperationTargetMaterialRandomnessRange = sphereMaterialDataCreator.OperationTargetMaterialRandomnessRange.ToVector2();
                MaterialHueRange = sphereMaterialDataCreator.MaterialColorRange.HueRange.ToVector2();
                MaterialSaturationRange = sphereMaterialDataCreator.MaterialColorRange.SaturationRange.ToVector2();
                MaterialValueRange = sphereMaterialDataCreator.MaterialColorRange.ValueRange.ToVector2();
                MaterialOpacityRange = sphereMaterialDataCreator.MaterialColorRange.OpacityRange.ToVector2();
                OperationTargetMaterialOpacityRange = sphereMaterialDataCreator.OperationTargetMaterialOpacityRange.ToVector2();
                TransparentOperationTargetRatio = sphereMaterialDataCreator.TransparentOperationTargetRatio.CurrentValue;
                WeightedBlendModes = sphereMaterialDataCreator.WeightedBlendModes.Weights.ToArray();
            }
        }

        public static SphereMaterialDataCreatorData ToData(this SphereMaterialDataCreator sphereMaterialDataCreator)
        {
            SphereMaterialDataCreatorData data = new (sphereMaterialDataCreator);
            return data;
        }

        public static void SetData(this SphereMaterialDataCreator sphereMaterialDataCreator, SphereMaterialDataCreatorData data)
        {
            sphereMaterialDataCreator.SetSeed(data.Seed);
            sphereMaterialDataCreator.OperationAreaMaterialRandomnessRange.Set(data.OperationAreaMaterialRandomnessRange);
            sphereMaterialDataCreator.OperationTargetMaterialRandomnessRange.Set(data.OperationTargetMaterialRandomnessRange);
            sphereMaterialDataCreator.MaterialColorRange.HueRange.Set(data.MaterialHueRange);
            sphereMaterialDataCreator.MaterialColorRange.SaturationRange.Set(data.MaterialSaturationRange);
            sphereMaterialDataCreator.MaterialColorRange.ValueRange.Set(data.MaterialValueRange);
            sphereMaterialDataCreator.MaterialColorRange.OpacityRange.Set(data.MaterialOpacityRange);
            sphereMaterialDataCreator.OperationTargetMaterialOpacityRange.Set(data.OperationTargetMaterialOpacityRange);
            sphereMaterialDataCreator.SetTransparentOperationTargetRatio(data.TransparentOperationTargetRatio);
            sphereMaterialDataCreator.WeightedBlendModes.SetWeights(data.WeightedBlendModes);
        }
    }
}