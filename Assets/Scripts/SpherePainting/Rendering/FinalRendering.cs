using UnityEngine;
using R3;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SpherePainting
{
    public class FinalRendering : MonoBehaviour
    {
        [SerializeField] private RenderResult m_RenderResult;
        private RenderTexture m_ResultTexture;
        [SerializeField] private Camera m_Camera;
        public Camera Camera => m_Camera;
        [SerializeField] private SphereDataListContainer m_SphereDatas;
        [SerializeField] private Renderer m_Renderer;
        [SerializeField] private ViewportRendering m_ViewportRendering;

        // 出力解像度
        private readonly ReactiveProperty<Vector2Int> m_Resolution = new ( new (1024, 1024));
        public ReadOnlyReactiveProperty<Vector2Int> Resolution => m_Resolution;
        public void SetResolution(Vector2Int resolution)
        {
            m_Resolution.Value = resolution;
        }
        // レンダリングサンプル数
        private readonly ReactiveProperty<uint> m_NumSamples = new (4);
        public ReadOnlyReactiveProperty<uint> NumSamples => m_NumSamples;
        public void SetNumSamples(uint samples)
        {
            m_NumSamples.Value = samples;
        }

        // タイルベースレンダリング時のタイルサイズ
        private readonly ReactiveProperty<Vector2Int> m_TileSize = new ( new (1024, 1024));
        public ReadOnlyReactiveProperty<Vector2Int> TileSize => m_TileSize;
        public void SetTileSize(Vector2Int tileSize)
        {
            m_TileSize.Value = tileSize;
        }

        public bool IsRendering { get; private set; } = false; // レンダリング中かどうか
        public event Action<RenderInformation> OnStartRendering; // レンダリング開始時に呼ぶ
        public event Action OnRenderComplete; // レンダリング完了時に呼ぶ

        // タイルベースレンダリングをするべきかどうか
        private bool ShouldTileBasedRendering()
        {
            return Mathf.Max(m_Resolution.Value.x, m_Resolution.Value.y) > Mathf.Min(2048, SystemInfo.maxTextureSize);
        }

        public async UniTask StartRendering(Action<float, int, int> onProgress = null, CancellationToken cancellationToken = default)
        {
            if(IsRendering) return;
            if(m_ViewportRendering.IsRendering) await m_ViewportRendering.CancelRender();
            IsRendering = true;
            InitRenderTextures();
            m_Renderer.SetSphereMaterialBuffer();
            m_Renderer.SetOperationTargetSpheresBuffer();
            m_Renderer.SetSpheresBuffer(m_Camera, false);

            int layerCount = m_SphereDatas.SphereDataCount;
            OnStartRendering?.Invoke(new RenderInformation(layerCount));

            Vector2Int tileSize = ShouldTileBasedRendering() ? m_TileSize.Value : m_Resolution.Value;
            try
            {
                for(int layer = 0; layer < layerCount; ++layer)
                {
                    bool isBottomLayer = layer == layerCount - 1;
                    for(uint currentSample = 0; currentSample < m_NumSamples.Value; ++currentSample)
                    {
                        m_Renderer.SetShaderParameters(m_Camera, m_Resolution.CurrentValue, currentSample, layer, isBottomLayer);

                        await m_Renderer.StartRendering(m_ResultTexture, tileSize, cancellationToken, (tileProgress) =>
                        {
                            float progress = (layer + ((currentSample + tileProgress) / m_NumSamples.Value)) / layerCount;
                            onProgress?.Invoke(progress, layer, layerCount);
                            m_RenderResult.CopyTexture(m_ResultTexture, layer);
                        });
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }
            }
            catch(OperationCanceledException)
            {
            }
            finally
            {
                IsRendering = false;
                onProgress?.Invoke(1.0f, layerCount-1, layerCount);
                OnRenderComplete?.Invoke();
            }
        }

        private void InitRenderTextures()
        {
            m_ResultTexture.ClearOut(); // テクスチャをクリア
            if (m_ResultTexture == null || m_RenderResult.IsEmpty.CurrentValue || IsResolutionChanged() || IsLayerCountChanged())
            {
                m_ResultTexture?.Release();

                m_ResultTexture = new RenderTexture(m_Resolution.Value.x, m_Resolution.Value.y, 0,
                                                    RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB)
                {
                    enableRandomWrite = true
                };
                m_ResultTexture.Create();

                m_RenderResult.Init(m_SphereDatas.SphereDataCount, m_Resolution.Value);
            }
        }

        // 最後のレンダリングから解像度が変更されたか
        private bool IsResolutionChanged()
        {
            Debug.Log("最後のレンダリングから解像度が変更された。");
            return m_RenderResult.Resolution != m_Resolution.Value;
        }

        // 最後のレンダリングからレイヤー数が変更されたか
        private bool IsLayerCountChanged()
        {
            Debug.Log("最後のレンダリングからレイヤー数が変更された。");
            return m_RenderResult.LayerCount != m_SphereDatas.SphereDataCount;
        }

        private void OnDestroy()
        {
            m_ResultTexture?.Release();
        }
    }
}