using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SpherePainting
{
    public static class SceneRenderSettingDataHandler
    {
        [Serializable]
        public class SceneRenderSettingData
        {
            [JsonProperty] public Vector4 BackgroundHSVColor { get; private set; }
            [JsonProperty] public bool DisplayBackground { get; private set; }
            [JsonProperty] public bool IsSpatialDistortionEnabled { get; private set;}
            [JsonProperty] public float RayRotationAroundZAxis { get; private set; }
            [JsonProperty] public Vector2 RayAmplitude { get; private set; }
            [JsonProperty] public Vector2 RayFrequency { get; private set; }
            [JsonProperty] public Vector2 RayPhaseOffset { get; private set; }

            public SceneRenderSettingData(){}

            public SceneRenderSettingData(SceneRenderSetting sceneRenderSetting)
            {
                BackgroundHSVColor = sceneRenderSetting.BackgroundHSVColor.Value;
                DisplayBackground = sceneRenderSetting.DisplayBackground.Value;
                IsSpatialDistortionEnabled = sceneRenderSetting.IsSpatialDistortionEnabled.CurrentValue;
                RayRotationAroundZAxis = sceneRenderSetting.RayRotationAroundZAxis.CurrentValue;
                RayAmplitude = sceneRenderSetting.RayAmplitude.CurrentValue;
                RayFrequency = sceneRenderSetting.RayFrequency.CurrentValue;
                RayPhaseOffset = sceneRenderSetting.RayPhaseOffset.CurrentValue;
            }
        }

        public static SceneRenderSettingData ToData(this SceneRenderSetting sceneRenderSetting)
        {
            SceneRenderSettingData data = new (sceneRenderSetting);
            return data;
        }

        public static void SetData(this SceneRenderSetting sceneRenderSetting, SceneRenderSettingData data)
        {
            sceneRenderSetting.BackgroundHSVColor.Value = data.BackgroundHSVColor;
            sceneRenderSetting.DisplayBackground.Value = data.DisplayBackground;
            sceneRenderSetting.SetSpatialDistortionEnabled(data.IsSpatialDistortionEnabled);
            sceneRenderSetting.SetRayRotationAroundZAxis(data.RayRotationAroundZAxis);
            sceneRenderSetting.SetRayAmplitude(data.RayAmplitude);
            sceneRenderSetting.SetRayFrequency(data.RayFrequency);
            sceneRenderSetting.SetRayPhaseOffset(data.RayPhaseOffset);
        }
    }
}