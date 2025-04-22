using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SpherePainting
{
    public static class CanvasDataHandler
    {
        [Serializable]
        public class CanvasData
        {
            [JsonProperty] public Vector2 Size { get; private set; }
            [JsonProperty] public float Depth { get; private set; }
            [JsonProperty] public Canvas.ShapeType ShapeType { get; private set; }
            [JsonProperty] public Vector2 NormalizedTextureOffset { get; private set; }
            [JsonProperty] public float TextureScaler { get; private set; }
            [JsonProperty] public bool IsLayerShuffleActive { get; private set; }
            [JsonProperty] public uint LayerShuffleSeed { get; private set; }
            [JsonProperty] public bool IsLayerFilterActive { get; private set; }
            [JsonProperty] public int ActiveFilteredLayer { get; private set; }

            public CanvasData(){}

            public CanvasData(Canvas canvas)
            {
                Size = canvas.Size.CurrentValue;
                Depth = canvas.Depth.CurrentValue;
                ShapeType = canvas.CurrentShapeType.CurrentValue;
                NormalizedTextureOffset = canvas.NormalizedTextureOffset.CurrentValue;
                TextureScaler = canvas.TextureScaler.CurrentValue;
                IsLayerShuffleActive = canvas.IsLayerShuffleActive.CurrentValue;
                LayerShuffleSeed = canvas.LayerShuffleSeed.CurrentValue;
                IsLayerFilterActive = canvas.IsLayerFilterActive.CurrentValue;
                ActiveFilteredLayer = canvas.ActiveFilteredLayer.CurrentValue;
            }
        }

        public static CanvasData ToData(this Canvas canvas)
        {
            CanvasData data = new CanvasData(canvas);
            return data;
        }

        public static void SetData(this Canvas canvas, CanvasData data)
        {
            canvas.SetSize(data.Size);
            canvas.SetDepth(data.Depth);
            canvas.ChangeCanvasShapeType(data.ShapeType);
            canvas.SetTextureOffset(data.NormalizedTextureOffset);
            canvas.SetTextureScale(data.TextureScaler);
            if(data.IsLayerShuffleActive)
            {
                canvas.ShuffleLayer(data.LayerShuffleSeed);
            }
            else
            {
                canvas.UnshuffleLayer();
            }
            if(data.IsLayerFilterActive)
            {
                canvas.FilterLayer(data.ActiveFilteredLayer);
            }
            else
            {
                canvas.ShowActiveLayers();
            }
        }
    }
}
