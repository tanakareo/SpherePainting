using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SpherePainting
{
    public class SphereVolumeExporter : MonoBehaviour
    {
        private static readonly int s_MaxDepth = 20;

        [SerializeField] private SphereVolumeRenderer m_SphereVolumeRenderer;
        [SerializeField] private FinalRendering m_FinalRendering;
        [SerializeField] private Canvas m_Canvas;
        [SerializeField] private SphereMaterialDataListContainer m_MaterialDatas;
        [SerializeField] private SphereDepthLayerIndicesCreator m_SphereDepthLayerIndicesCreator;
        [SerializeField] private SphereDataListContainer m_SphereDatas;
        [SerializeField] private FileExporter m_FileExporter;
        private RenderTexture m_ResultTexture;

        private string CreateLayerFolderName(int layerIndex)
        {
            return $"Layer{layerIndex + 1:00}";
        }

        private string CreateSphereVolumeFileName(int sphereIndex, int depth)
        {
            return $"Sphere{sphereIndex:00}_{m_MaterialDatas.GetData(sphereIndex).BlendMode.ToJapanese()}_Depth{depth + 1:00}";
        }

        public async UniTask StartRenderingAndExport(Action<float, int, int> onProgress = null, CancellationToken cancellationToken = default)
        {
            // 初期化処理
            InitRenderTextures();
            m_SphereVolumeRenderer.SetShaderParameters();
            int[] layerIndices = m_SphereDepthLayerIndicesCreator.Create(m_FinalRendering.Camera.transform);

            try
            {
                for(int sphereIndex = 0; sphereIndex < m_SphereDatas.SphereDataCount; ++sphereIndex)
                {
                    for(int depth = 0; depth < s_MaxDepth; ++depth)
                    {
                        int hitCount = await m_SphereVolumeRenderer.StartRendering(m_ResultTexture, sphereIndex, depth, cancellationToken,
                        tileProgress =>
                        {
                            onProgress?.Invoke( ((tileProgress + depth) / s_MaxDepth + sphereIndex) / m_SphereDatas.SphereDataCount, sphereIndex, m_SphereDatas.SphereDataCount);
                        });
                        cancellationToken.ThrowIfCancellationRequested();

                        float threshold = Mathf.Max(0.0f, m_ResultTexture.width * m_ResultTexture.height * 0.00001f);
                        if(hitCount <= threshold) break;
                        string folderPath = Path.Combine(m_FileExporter.ExportFolderPath.CurrentValue, CreateLayerFolderName(layerIndices[sphereIndex]));
                        string fileName = CreateSphereVolumeFileName(sphereIndex, depth);
                        m_ResultTexture.ExportAsPNG(folderPath, fileName);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // キャンセル例外はそのまま無視
                Debug.Log("OperationCanceledException");
            }
            catch (Exception ex)
            {
                // その他の例外も無視（必要に応じてログ出力等の処理を追加してください）
                Debug.LogError(ex);
            }
            finally
            {
                onProgress?.Invoke(1.0f, m_SphereDatas.SphereDataCount - 1, m_SphereDatas.SphereDataCount);
            }
        }

        private void InitRenderTextures()
        {
            if(m_ResultTexture != null) m_ResultTexture.ClearOut();
            if (m_ResultTexture == null || IsResolutionChanged())
            {
                m_ResultTexture?.Release();

                // Get a render target for Ray Tracing
                m_ResultTexture = new RenderTexture(m_FinalRendering.Resolution.CurrentValue.x, m_FinalRendering.Resolution.CurrentValue.y, 0,
                                                    RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB)
                {
                    enableRandomWrite = true
                };
                m_ResultTexture.Create();
            }
        }

        private bool IsResolutionChanged()
        {
            return m_ResultTexture.width != m_FinalRendering.Resolution.CurrentValue.x || m_ResultTexture.height != m_FinalRendering.Resolution.CurrentValue.y;
        }

        private void OnDestroy()
        {
            m_ResultTexture?.Release();
        }
    }
}