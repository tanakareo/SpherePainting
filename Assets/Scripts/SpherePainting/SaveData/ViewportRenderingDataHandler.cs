using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace SpherePainting
{
    public static  class ViewportRenderingDataHandler
    {
        [Serializable]
        public class ViewportRenderingData
        {
            [JsonProperty] public Vector2Int Resolution { get; private set; }
            [JsonProperty] public uint NumSamples { get; private set; }
            [JsonProperty] public bool IsTileBasedRenderingEnabled { get; private set; }
            [JsonProperty] public Vector2Int TileSize { get; private set; }

            public ViewportRenderingData(){}

            public ViewportRenderingData(ViewportRendering viewportRendering)
            {
                Resolution = viewportRendering.Resolution.CurrentValue;
                NumSamples = viewportRendering.NumSamples.CurrentValue;
                IsTileBasedRenderingEnabled = viewportRendering.IsTileBasedRenderingEnabled.CurrentValue;
                TileSize = viewportRendering.TileSize.CurrentValue;
            }
        }

        public static ViewportRenderingData ToData(this ViewportRendering viewportRendering)
        {
            ViewportRenderingData data = new (viewportRendering);
            return data;
        }

        public static void SetData(this ViewportRendering viewportRendering, ViewportRenderingData data)
        {
            viewportRendering.SetResolution(data.Resolution).Forget();
            viewportRendering.SetNumSamples(data.NumSamples);
            viewportRendering.SetTileBasedRenderingEnabled(data.IsTileBasedRenderingEnabled);
            viewportRendering.SetTileSize(data.TileSize);
        }
    }
}
