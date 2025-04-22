using Unity.Mathematics;
using UnityEngine;

namespace SpherePainting
{
    using ShapeType = Canvas.ShapeType;
    public class ExportableCanvas : MonoBehaviour
    {
        [SerializeField] private Canvas m_Canvas;
        [SerializeField] private CanvasMaterialSetting m_CanvasMaterialSetting;

        public int[] OriginalLayerIndices => m_Canvas.OriginalLayerIndices;
        public int[] CurrentLayerIndices => m_Canvas.CurrentLayerIndices;
        public ShapeType ShapeType => m_Canvas.CurrentShapeType.CurrentValue;
        public bool IsLayerShuffleActive => m_Canvas.IsLayerShuffleActive.CurrentValue;
        public float3 BaseHSVColor => m_CanvasMaterialSetting.BaseHSVColor;

        public Vector2 GetLayerPixelOffset(Vector2Int resolution)
        {
            return m_Canvas.CurrentTextureOffset.CurrentValue * resolution;
        }

        public Vector2 GetSizeInPixels(Vector2Int resolution)
        {
            return m_Canvas.CurrentTextureScale.CurrentValue * resolution;
        }
    }
}