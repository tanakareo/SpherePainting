using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpherePainting
{
    public class SphereVolumeRenderer : MonoBehaviour
    {
        [SerializeField] private ComputeShader m_ComputeShader;
        [SerializeField] private SphereRenderSetting m_SphereRenderSetting;
        [SerializeField] private SceneRenderSetting m_SceneRenderSetting;
        [SerializeField] private SphereLayerIndicesGraphicsBufferCreator m_SphereLayerIndicesGraphicsBufferCreator;
        [SerializeField] private SpheresGraphicsBufferCreator m_SpheresGraphicsBufferCreator;
        [SerializeField] private SphereMaterialGraphicsBufferCreator m_SphereMaterialGraphicsBufferCreator;
        [SerializeField] private FinalRendering m_FinalRendering;

        public bool IsRendering { get; private set; } = false;
        private RenderTexture m_TileRenderTexture;
        private ITileAreaUpdater m_TileAreaUpdater;
        private GraphicsBuffer m_HitCountBuffer;

        void Awake()
        {
            m_HitCountBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 1, sizeof(int));
        }

        void OnDestroy()
        {
            m_HitCountBuffer?.Dispose();
        }

        private void UpdateTileAreaUpdater(Vector2Int resolution, Vector2Int tileSize)
        {
            if (m_TileAreaUpdater == null || m_TileAreaUpdater.Resolution != resolution || m_TileAreaUpdater.TileSize != tileSize)
            {
                m_TileAreaUpdater = new SpiralTileAreaUpdater(resolution, tileSize);
                return;
            }

            m_TileAreaUpdater.Reset();
        }

        private void UpdateTileRenderTextureSize(Vector2Int tileSize, int volumeDepth)
        {
            if(m_TileRenderTexture == null)
            {
                m_TileRenderTexture = new RenderTexture(tileSize.x, tileSize.y, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB)
                {
                    enableRandomWrite = true,
                };
                m_TileRenderTexture.Create();
                return;
            }
            if(m_TileRenderTexture.width == tileSize.x && m_TileRenderTexture.height == tileSize.y) return;
            m_TileRenderTexture.Release();
            m_TileRenderTexture.width = tileSize.x;
            m_TileRenderTexture.height = tileSize.y;
            m_TileRenderTexture.Create();
        }

        public void SetShaderParameters()
        {
            m_ComputeShader.SetMatrix("_CameraToWorld", m_FinalRendering.Camera.cameraToWorldMatrix);
            m_ComputeShader.SetMatrix("_CameraInverseProjection", m_FinalRendering.Camera.projectionMatrix.inverse);
            m_ComputeShader.SetBool("_IsOrthographic", m_FinalRendering.Camera.orthographic);
            
            m_ComputeShader.SetVector("_Resolution", (Vector2)m_FinalRendering.Resolution.CurrentValue);
            
            m_ComputeShader.SetBool("_DisplayBackFace", m_SphereRenderSetting.DisplayBackFace.CurrentValue);
            m_ComputeShader.SetFloat("_SphereBlendStrength", m_SphereRenderSetting.SphereBlendStrength.CurrentValue);
            m_ComputeShader.SetFloat("_OperationTargetBlendStrength", m_SphereRenderSetting.OperationTargetBlendStrength.CurrentValue);
            m_ComputeShader.SetFloat("_SphereMaterialBlendStrength", m_SphereRenderSetting.SphereMaterialBlendStrength.CurrentValue);
            m_ComputeShader.SetFloat("_OperationTargetMaterialBlendStrength", m_SphereRenderSetting.OperationTargetMaterialBlendStrength.CurrentValue);
            m_ComputeShader.SetFloat("_OperationSmoothness", m_SphereRenderSetting.OperationSmoothness.CurrentValue);
            m_ComputeShader.SetFloat("_OperationMaterialSmoothness", m_SphereRenderSetting.OperationMaterialSmoothness.CurrentValue);
            
            m_ComputeShader.SetBool("_IsSpatialDistortionEnabled", m_SceneRenderSetting.IsSpatialDistortionEnabled.CurrentValue);
            m_ComputeShader.SetFloat("_RayRotationAroundZAxis", m_SceneRenderSetting.RayRotationAroundZAxis.CurrentValue);
            m_ComputeShader.SetVector("_RayAmplitude", m_SceneRenderSetting.RayAmplitude.CurrentValue);
            m_ComputeShader.SetVector("_RayFrequency", m_SceneRenderSetting.RayFrequency.CurrentValue);
            m_ComputeShader.SetVector("_RayPhaseOffset", m_SceneRenderSetting.RayPhaseOffset.CurrentValue);

            m_ComputeShader.SetBuffer(1, "_Spheres", m_SpheresGraphicsBufferCreator.SpheresBuffer);
            m_ComputeShader.SetInt("_NumSpheres", m_SpheresGraphicsBufferCreator.SpheresBuffer.count);
            m_ComputeShader.SetBuffer(1, "_OperationTargetSpheres", m_SpheresGraphicsBufferCreator.OperationTargetSpheresBuffer);
            m_ComputeShader.SetBuffer(1, "_SphereMaterials", m_SphereMaterialGraphicsBufferCreator.Buffer);
            m_SphereLayerIndicesGraphicsBufferCreator.UpdateDepthLayerBuffer(m_FinalRendering.Camera.transform);
            m_ComputeShader.SetBuffer(1, "_SphereLayerIndices", m_SphereLayerIndicesGraphicsBufferCreator.DepthLayerBuffer);        
        }

        public async UniTask<int> StartRendering(RenderTexture renderTexture, int renderTargetSphereIndex, int renderTargetDepth, CancellationToken cancellationToken = default, Action<float> onProgress = null)
        {
            IsRendering = true;
            
            UpdateTileAreaUpdater(m_FinalRendering.Resolution.CurrentValue, m_FinalRendering.TileSize.CurrentValue);
            m_ComputeShader.SetInt("_RenderTargetSphereIndex", renderTargetSphereIndex);
            m_ComputeShader.SetInt("_RenderTargetDepth", renderTargetDepth);
            m_HitCountBuffer.SetData(new int[]{0});
            m_ComputeShader.SetBuffer(1, "_HitCount", m_HitCountBuffer);
            int[] hitCount = new int[1]{0};
            onProgress?.Invoke(0.0f);

            try
            {
                while (m_TileAreaUpdater.IsCompleted() == false)
                {
                    await RenderNextTile(renderTexture, m_TileAreaUpdater.CurrentTileArea, cancellationToken);
                    onProgress?.Invoke(m_TileAreaUpdater.Progress);
                    cancellationToken.ThrowIfCancellationRequested();
                    m_TileAreaUpdater.Update();
                }
            }
            catch(OperationCanceledException)
            {
            }
            catch(Exception)
            {
            }
            finally
            {
                onProgress?.Invoke(1.0f);
                IsRendering = false;

                m_HitCountBuffer.GetData(hitCount);
            }
            return hitCount[0];
        }

        private async UniTask RenderNextTile(RenderTexture renderTexture, TileArea tileArea, CancellationToken cancellationToken)
        {
            try
            {
                UpdateTileRenderTextureSize(tileArea.Size, renderTexture.volumeDepth);

                Graphics.CopyTexture(renderTexture, 0, 0, tileArea.Offset.x, tileArea.Offset.y, tileArea.Size.x, tileArea.Size.y,
                                     m_TileRenderTexture, 0, 0, 0, 0);

                m_ComputeShader.SetTexture(1, "_Result", m_TileRenderTexture);
                m_ComputeShader.SetVector("_TileOffset", (Vector2)tileArea.Offset);

                // タイルサイズに基づいてスレッドグループ数を計算
                int threadGroupsX = Mathf.CeilToInt(tileArea.Size.x / 8.0f);
                int threadGroupsY = Mathf.CeilToInt(tileArea.Size.y / 8.0f);

                for(int currentSample = 0; currentSample < m_FinalRendering.NumSamples.CurrentValue; ++currentSample)
                {
                    m_ComputeShader.SetInt("_CurrentSample", currentSample);
                    Vector2 pixelOffset = new Vector2(Random.value - 0.5f, Random.value - 0.5f);
                    m_ComputeShader.SetVector("_PixelOffset", pixelOffset);
                    m_ComputeShader.Dispatch(1, threadGroupsX, threadGroupsY, 1);
                    await UniTask.Yield(cancellationToken);
                }

                Graphics.CopyTexture(m_TileRenderTexture, 0, 0, 0, 0, tileArea.Size.x, tileArea.Size.y,
                                    renderTexture, 0, 0, tileArea.Offset.x, tileArea.Offset.y);
            }
            catch(OperationCanceledException)
            {
            }
            catch(Exception)
            {
            }
            finally
            {
            }

        }
    }
}