using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SpherePainting
{
    public static  class FinalRenderingDataHandler
    {
        [Serializable]
        public class FinalRenderingData
        {
            [JsonProperty] public Vector2Int Resolution { get; private set; }
            [JsonProperty] public uint NumSamples { get; private set; }
            [JsonProperty] public bool IsTileBasedRenderingEnabled { get; private set; }
            [JsonProperty] public Vector2Int TileSize { get; private set; }

            public FinalRenderingData(){}

            public FinalRenderingData(FinalRendering finalRendering)
            {
                Resolution = finalRendering.Resolution.CurrentValue;
                NumSamples = finalRendering.NumSamples.CurrentValue;
                TileSize = finalRendering.TileSize.CurrentValue;
            }
        }

        public static FinalRenderingData ToData(this FinalRendering finalRendering)
        {
            FinalRenderingData data = new (finalRendering);
            return data;
        }

        public static void SetData(this FinalRendering finalRendering, FinalRenderingData data)
        {
            finalRendering.SetResolution(data.Resolution);
            finalRendering.SetNumSamples(data.NumSamples);
            finalRendering.SetTileSize(data.TileSize);
        }
    }
}
