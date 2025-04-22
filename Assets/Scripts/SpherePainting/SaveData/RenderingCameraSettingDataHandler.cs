using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace SpherePainting
{
    public static class RenderingCameraSettingDataHandler
    {
        [Serializable]
        public class RenderingCameraSettingData
        {
            [JsonProperty] public bool IsOrthographic { get; private set; }
            [JsonProperty] public float OrthographicSize { get; private set; }
            [JsonProperty] public float FieldOfView { get; private set; }
            [JsonProperty] public Vector3 CameraPosition { get; private set; }
            [JsonProperty] public Vector3 CameraRotation { get; private set; }

            public RenderingCameraSettingData(){}

            public RenderingCameraSettingData(RenderingCameraSetting renderingCameraSetting)
            {
                IsOrthographic = renderingCameraSetting.IsOrthographic;
                OrthographicSize = renderingCameraSetting.OrthographicSize;
                FieldOfView = renderingCameraSetting.FieldOfView;
                CameraPosition = renderingCameraSetting.CameraPosition;
                CameraRotation = renderingCameraSetting.CameraRotation;
            }
        }

        public static RenderingCameraSettingData ToData(this RenderingCameraSetting renderingCameraSetting)
        {
            RenderingCameraSettingData data = new (renderingCameraSetting);
            return data;
        }

        public static void SetData(this RenderingCameraSetting renderingCameraSetting, RenderingCameraSettingData data)
        {
            renderingCameraSetting.SetOrthographicFlag(data.IsOrthographic);
            renderingCameraSetting.SetOrthographicSize(data.OrthographicSize);
            renderingCameraSetting.SetFieldOfView(data.FieldOfView);
            renderingCameraSetting.SetCameraPosition(data.CameraPosition);
            renderingCameraSetting.SetCameraRotation(data.CameraRotation);
        }
    }
}