using System;
using Newtonsoft.Json;

namespace SpherePainting
{
    public static class SphereRenderSettingDataHandler
    {
        [Serializable]
        public class SphereRenderSettingData
        {
            [JsonProperty] public float SphereBlendStrength { get; private set; }
            [JsonProperty] public float OperationTargetBlendStrength { get; private set; }
            [JsonProperty] public float SphereMaterialBlendStrength { get; private set; }
            [JsonProperty] public float OperationTargetMaterialBlendStrength { get; private set; }
            [JsonProperty] public float OperationSmoothness { get; private set; }
            [JsonProperty] public float OperationMaterialSmoothness { get; private set; }

            public SphereRenderSettingData(){}

            public SphereRenderSettingData(SphereRenderSetting sphereRenderSetting)
            {
                SphereBlendStrength = sphereRenderSetting.SphereBlendStrength.CurrentValue;
                OperationTargetBlendStrength = sphereRenderSetting.OperationTargetBlendStrength.CurrentValue;
                SphereMaterialBlendStrength = sphereRenderSetting.SphereMaterialBlendStrength.CurrentValue;
                OperationTargetMaterialBlendStrength = sphereRenderSetting.OperationTargetMaterialBlendStrength.CurrentValue;
                OperationSmoothness = sphereRenderSetting.OperationSmoothness.CurrentValue;
                OperationMaterialSmoothness = sphereRenderSetting.OperationMaterialSmoothness.CurrentValue;
            }
        }

        public static SphereRenderSettingData ToData(this SphereRenderSetting sphereRenderSetting)
        {
            SphereRenderSettingData data = new (sphereRenderSetting);
            return data;
        }

        public static void SetData(this SphereRenderSetting sphereRenderSetting, SphereRenderSettingData data)
        {
            sphereRenderSetting.SetSphereBlendStrength(data.SphereBlendStrength);
            sphereRenderSetting.SetOperationTargetBlendStrength(data.OperationTargetBlendStrength);
            sphereRenderSetting.SetSphereMaterialBlendStrength(data.SphereMaterialBlendStrength);
            sphereRenderSetting.SetOperationTargetMaterialBlendStrength(data.OperationTargetMaterialBlendStrength);
            sphereRenderSetting.SetOperationSmoothness(data.OperationSmoothness);
            sphereRenderSetting.SetOperationMaterialSmoothness(data.OperationMaterialSmoothness);
        }
    }
}