using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpherePainting
{
    public class Renderer : MonoBehaviour
    {
        [SerializeField] private ComputeShader m_ComputeShader;
        [SerializeField] private SphereRenderSetting m_SphereRenderSetting;
        [SerializeField] private SceneRenderSetting m_SceneRenderSetting;
        [SerializeField] private SphereLayerIndicesGraphicsBufferCreator m_SphereLayerIndicesGraphicsBufferCreator;
        [SerializeField] private SpheresGraphicsBufferCreator m_SpheresGraphicsBufferCreator;
        [SerializeField] private SphereMaterialGraphicsBufferCreator m_SphereMaterialGraphicsBufferCreator;
        public bool IsRendering { get; private set; } = false;
        private RenderTexture m_TileRenderTexture;
        private ITileAreaUpdater m_TileAreaUpdater;

        private void UpdateTileAreaUpdater(Vector2Int resolution, Vector2Int tileSize)
        {
            if (m_TileAreaUpdater == null || m_TileAreaUpdater.Resolution != resolution || m_TileAreaUpdater.TileSize != tileSize)
            {
                m_TileAreaUpdater = new SpiralTileAreaUpdater(resolution, tileSize);
                return;
            }

            m_TileAreaUpdater.Reset();
        }

        private void UpdateTileRenderTextureSize(Vector2Int tileSize)
        {
            if(m_TileRenderTexture == null)
            {
                m_TileRenderTexture = new RenderTexture(tileSize.x, tileSize.y, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB)
                {
                    enableRandomWrite = true,
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp,
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

        public void SetShaderParameters(Camera camera, Vector2Int resolution, uint currentSample = 0, int renderTargetLayer = 0, bool isBottomLayer = true)
        {
            m_ComputeShader.SetMatrix("_CameraToWorld", camera.cameraToWorldMatrix);
            m_ComputeShader.SetMatrix("_CameraInverseProjection", camera.projectionMatrix.inverse);
            m_ComputeShader.SetBool("_IsOrthographic", camera.orthographic);
            
            m_ComputeShader.SetInt("_CurrentSample", (int)currentSample);
            Vector2 pixelOffset = (currentSample == 0) ? Vector2.zero : new Vector2(Random.value - 0.5f, Random.value - 0.5f);
            m_ComputeShader.SetVector("_PixelOffset", pixelOffset);
            m_ComputeShader.SetVector("_Resolution", (Vector2)resolution);
            
            m_ComputeShader.SetBool("_DisplayBackFace", m_SphereRenderSetting.DisplayBackFace.CurrentValue);
            m_ComputeShader.SetFloat("_SphereBlendStrength", m_SphereRenderSetting.SphereBlendStrength.CurrentValue);
            m_ComputeShader.SetFloat("_OperationTargetBlendStrength", m_SphereRenderSetting.OperationTargetBlendStrength.CurrentValue);
            m_ComputeShader.SetFloat("_SphereMaterialBlendStrength", m_SphereRenderSetting.SphereMaterialBlendStrength.CurrentValue);
            m_ComputeShader.SetFloat("_OperationTargetMaterialBlendStrength", m_SphereRenderSetting.OperationTargetMaterialBlendStrength.CurrentValue);
            m_ComputeShader.SetFloat("_OperationSmoothness", m_SphereRenderSetting.OperationSmoothness.CurrentValue);
            m_ComputeShader.SetFloat("_OperationMaterialSmoothness", m_SphereRenderSetting.OperationMaterialSmoothness.CurrentValue);
            
            m_ComputeShader.SetBool("_IsBottomLayer", isBottomLayer);
            bool displayBackground = m_SceneRenderSetting.DisplayBackground.CurrentValue && isBottomLayer;
            m_ComputeShader.SetBool("_DisplayBackground", displayBackground);
            m_ComputeShader.SetVector("_BackgroundColor", m_SceneRenderSetting.BackgroundRGBColor);
            
            m_ComputeShader.SetBool("_IsSpatialDistortionEnabled", m_SceneRenderSetting.IsSpatialDistortionEnabled.CurrentValue);
            m_ComputeShader.SetFloat("_RayRotationAroundZAxis", m_SceneRenderSetting.RayRotationAroundZAxis.CurrentValue);
            m_ComputeShader.SetVector("_RayAmplitude", m_SceneRenderSetting.RayAmplitude.CurrentValue);
            m_ComputeShader.SetVector("_RayFrequency", m_SceneRenderSetting.RayFrequency.CurrentValue);
            m_ComputeShader.SetVector("_RayPhaseOffset", m_SceneRenderSetting.RayPhaseOffset.CurrentValue);
            m_ComputeShader.SetInt("_RenderTargetLayerIndex", renderTargetLayer);
        }

        public void SetSpheresBuffer(Camera camera, bool isSingleLayer = true)
        {
            m_ComputeShader.SetBuffer(0, "_Spheres", m_SpheresGraphicsBufferCreator.SpheresBuffer);
            m_ComputeShader.SetInt("_NumSpheres", m_SpheresGraphicsBufferCreator.SpheresBuffer.count);

            if(isSingleLayer)
            {
                m_SphereLayerIndicesGraphicsBufferCreator.UpdateSingleLayerBuffer();
                m_ComputeShader.SetBuffer(0, "_SphereLayerIndices", m_SphereLayerIndicesGraphicsBufferCreator.SingleLayerBuffer);
            }
            else
            {
                m_SphereLayerIndicesGraphicsBufferCreator.UpdateDepthLayerBuffer(camera.transform);
                m_ComputeShader.SetBuffer(0, "_SphereLayerIndices", m_SphereLayerIndicesGraphicsBufferCreator.DepthLayerBuffer);
            }
        }

        public void SetOperationTargetSpheresBuffer()
        {
            m_ComputeShader.SetBuffer(0, "_OperationTargetSpheres", m_SpheresGraphicsBufferCreator.OperationTargetSpheresBuffer);
        }

        public void SetSphereMaterialBuffer()
        {
            m_ComputeShader.SetBuffer(0, "_SphereMaterials", m_SphereMaterialGraphicsBufferCreator.Buffer);
        }

        public void StartRendering(RenderTexture renderTexture)
        {
            m_ComputeShader.SetTexture(0, "_Result", renderTexture);
            m_ComputeShader.SetVector("_TileOffset", Vector2.zero);

            // タイルサイズに基づいてスレッドグループ数を計算
            int threadGroupsX = Mathf.CeilToInt(renderTexture.width / 8.0f);
            int threadGroupsY = Mathf.CeilToInt(renderTexture.height / 8.0f);

            m_ComputeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        }

        public async UniTask StartRendering(RenderTexture renderTexture, Vector2Int tileSize, CancellationToken cancellationToken = default, Action<float> onProgress = null)
        {
            IsRendering = true;
            
            Vector2Int resolution = new Vector2Int(renderTexture.width, renderTexture.height);
            UpdateTileAreaUpdater(resolution, tileSize);
            try
            {
                while (m_TileAreaUpdater.IsCompleted() == false)
                {
                    RenderNextTile(renderTexture, m_TileAreaUpdater.CurrentTileArea);
                    onProgress?.Invoke(m_TileAreaUpdater.Progress);
                    await UniTask.Yield(cancellationToken);
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
                IsRendering = false;
            }
        }

        private void RenderNextTile(RenderTexture renderTexture, TileArea tileArea)
        {
            UpdateTileRenderTextureSize(tileArea.Size);

            Graphics.CopyTexture(renderTexture, 0, 0, tileArea.Offset.x, tileArea.Offset.y, tileArea.Size.x, tileArea.Size.y,
                                 m_TileRenderTexture, 0, 0, 0, 0);

            // 現在のタイルの範囲を ComputeShader に設定
            m_ComputeShader.SetTexture(0, "_Result", m_TileRenderTexture);
            m_ComputeShader.SetVector("_TileOffset", (Vector2)tileArea.Offset);

            // タイルサイズに基づいてスレッドグループ数を計算
            int threadGroupsX = Mathf.CeilToInt(tileArea.Size.x / 8.0f);
            int threadGroupsY = Mathf.CeilToInt(tileArea.Size.y / 8.0f);

            m_ComputeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

            Graphics.CopyTexture(m_TileRenderTexture, 0, 0, 0, 0, tileArea.Size.x, tileArea.Size.y,
                                 renderTexture, 0, 0, tileArea.Offset.x, tileArea.Offset.y);
        }
    }
}