using UnityEngine;
using SkiaSharp;
using System.IO;

namespace SpherePainting
{
    public class CanvasSVGExporter : MonoBehaviour
    {
        [SerializeField] private ExportableCanvas m_ExportableCanvas;
        [SerializeField] private RenderResult m_RenderResult;

        public void Export(string[] exportedImagePaths, string folderPath, string fileName, bool shouldCreateOnlyShuffledLayers = false)
        {
            Vector2 viewportSize = m_ExportableCanvas.GetSizeInPixels(m_RenderResult.Resolution);
            string filePath = Path.Combine(folderPath, $"{fileName}.svg");

            using (var stream = new SKFileWStream(filePath))
            using (var document = SKSvgCanvas.Create(new SKRect(0, 0, viewportSize.x, viewportSize.y), stream))
            {
                ApplyClipPath(document);

                if (m_ExportableCanvas.IsLayerShuffleActive == false || shouldCreateOnlyShuffledLayers)
                {
                    DrawLayers(document, m_ExportableCanvas.OriginalLayerIndices, viewportSize, exportedImagePaths);
                }
                if (m_ExportableCanvas.IsLayerShuffleActive)
                {
                    DrawLayers(document, m_ExportableCanvas.CurrentLayerIndices, viewportSize, exportedImagePaths);
                }
            }
        }

        private void DrawLayers(SKCanvas canvas, int[] layerIndices, Vector2 viewportSize, string[] exportedImagePaths)
        {
            Vector2 layerOffset = m_ExportableCanvas.GetLayerPixelOffset(m_RenderResult.Resolution);
            Vector2 layerPosition = new Vector2(-layerOffset.x, -(m_RenderResult.Resolution.y - layerOffset.y - viewportSize.y));
            
            for (int i = exportedImagePaths.Length - 1; i >= 0; --i)
            {
                string imagePath = exportedImagePaths[layerIndices[i]];
                if (!File.Exists(imagePath)) continue;
                using SKImage image = SKImage.FromEncodedData(imagePath);
                canvas.DrawImage(image, layerPosition.x, layerPosition.y, new SKPaint());
            }
        }

        private void ApplyClipPath(SKCanvas canvas)
        {
            Vector2 size = m_ExportableCanvas.GetSizeInPixels(m_RenderResult.Resolution);
            using SKPath clipPath = new SKPath();
            SKRect bounds = new SKRect(0, 0, size.x, size.y);
            switch(m_ExportableCanvas.ShapeType)
            {
                case Canvas.ShapeType.ELLIPSE:
                    clipPath.AddOval(bounds);
                break;
                case Canvas.ShapeType.RECTANGLE:
                    clipPath.AddRect(bounds);
                break;
            }
            canvas.ClipPath(clipPath);
        }
    }
}