using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SpherePainting
{
    public static class CanvasMaterialSettingDataHandler
    {
        [Serializable]
        public class CanvasMaterialSettingData
        {
            [JsonProperty] public Vector3 BaseHSVColor { get; private set; }

            public CanvasMaterialSettingData(){}

            public CanvasMaterialSettingData(CanvasMaterialSetting canvasMaterialSetting)
            {
                BaseHSVColor = canvasMaterialSetting.BaseHSVColor;
            }
        }

        public static CanvasMaterialSettingData ToData(this CanvasMaterialSetting canvasMaterialSetting)
        {
            CanvasMaterialSettingData data = new (canvasMaterialSetting);
            return data;
        }

        public static void SetData(this CanvasMaterialSetting canvasMaterialSetting, CanvasMaterialSettingData data)
        {
            canvasMaterialSetting.SetBaseMaterialColor(data.BaseHSVColor);
        }
    }
}
