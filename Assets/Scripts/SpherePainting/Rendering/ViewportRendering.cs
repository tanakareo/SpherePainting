using System;
using System.Threading;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace SpherePainting
{
    public class ViewportRendering : MonoBehaviour
    {
        [SerializeField, SerializedDictionary("Type", "Camera")] private SerializedDictionary<CameraType, Camera> m_Cameras;
        private Camera CurrentCamera => m_Cameras[CurrentCameraType];
        [SerializeField] private RenderTexture m_ResultTexture;
        [SerializeField] private ComputeShader m_ViewportRenderingShader;
        [SerializeField] private FinalRendering m_FinalRendering;
        [SerializeField] private Renderer m_Renderer;
        [SerializeField] private SpheresGraphicsBufferCreator m_SpheresGraphicsBufferCreator;
        [SerializeField] private SphereMaterialGraphicsBufferCreator m_SphereMaterialGraphicsBufferCreator;
        public CameraType CurrentCameraType { get; private set; } = CameraType.RENDERING;
        private bool m_IsRenderRequired;

        private readonly ReactiveProperty<Vector2Int> m_Resolution = new ( new (1024, 1024));
        public ReadOnlyReactiveProperty<Vector2Int> Resolution => m_Resolution;
        public async UniTask SetResolution(Vector2Int resolution)
        {
            if (m_Resolution.Value == resolution) return; // 変更がなければ何もしない

            await CancelRender();

            m_Resolution.Value = resolution;

            m_ResultTexture.Release();
            m_ResultTexture.width = m_Resolution.Value.x;
            m_ResultTexture.height = m_Resolution.Value.y;
            m_ResultTexture.Create();

            RequireRendering();
        }
        private readonly ReactiveProperty<uint> m_NumSamples = new (1);
        public ReadOnlyReactiveProperty<uint> NumSamples => m_NumSamples;
        public void SetNumSamples(uint samples)
        {
            m_NumSamples.Value = samples;
            RequireRendering();
        }

        private readonly ReactiveProperty<bool> m_IsTileBasedRenderingEnabled = new (false);
        public ReadOnlyReactiveProperty<bool> IsTileBasedRenderingEnabled => m_IsTileBasedRenderingEnabled;
        public void SetTileBasedRenderingEnabled(bool enabled)
        {
            m_IsTileBasedRenderingEnabled.Value = enabled;
            RequireRendering();
        }
        
        private readonly ReactiveProperty<Vector2Int> m_TileSize = new ( new (1024, 1024));
        public ReadOnlyReactiveProperty<Vector2Int> TileSize => m_TileSize;
        public void SetTileSize(Vector2Int tileSize)
        {
            m_TileSize.Value = tileSize;
            RequireRendering();
        }

        private CancellationTokenSource m_CancellationTokenSource;
        public event Action OnRenderProgress;
        public bool IsRendering { get; private set; } = false;

        private bool ShouldTileBasedRendering()
        {
            return Mathf.Max(m_Resolution.Value.x, m_Resolution.Value.y) > Mathf.Min(2048, SystemInfo.maxTextureSize);
        }

        private void Awake()
        {
            SwitchCamera(CurrentCameraType);
            m_FinalRendering.OnRenderComplete += SetSpheresBuffer;
            InitResultTexture();
            Resolution.Subscribe(v =>
            {
                if(ShouldTileBasedRendering() == false) return;

                SetTileBasedRenderingEnabled(true);
            }).AddTo(this);

            IsTileBasedRenderingEnabled.Subscribe(v =>
            {
                if(v) return;
                if(ShouldTileBasedRendering() == false) return;
                SetTileBasedRenderingEnabled(true);
            }).AddTo(this);
        }

        private void OnDestroy()
        {
            if(m_CancellationTokenSource == null) return;
            
            m_CancellationTokenSource.Cancel();
            m_CancellationTokenSource.Dispose();
        }

        private void InitResultTexture()
        {
            m_ResultTexture.Release();
            m_ResultTexture.width = m_Resolution.Value.x;
            m_ResultTexture.height = m_Resolution.Value.y;
            m_ResultTexture.Create();
        }

        private void Start()
        {
            SetSpheresBuffer();
            SetOperationTargetSpheresBuffer();
            SetSphereMaterialBuffer();
            m_SpheresGraphicsBufferCreator.OnSpheresBufferCreated += SetSpheresBuffer;
            m_SpheresGraphicsBufferCreator.OnOperationTargetSpheresBufferCreated += SetOperationTargetSpheresBuffer;
            m_SphereMaterialGraphicsBufferCreator.OnBufferCreated += SetSphereMaterialBuffer;
        }

        private void SetSphereMaterialBuffer()
        {
            m_Renderer.SetSphereMaterialBuffer();
            RequireRendering();
        }

        private void SetOperationTargetSpheresBuffer()
        {
            m_Renderer.SetOperationTargetSpheresBuffer();
            RequireRendering();
        }

        private void SetSpheresBuffer()
        {
            m_Renderer.SetSpheresBuffer(CurrentCamera);
            RequireRendering();
        }

        private void Update()
        {
            if(m_FinalRendering.IsRendering) return;

            if(CurrentCameraType == CameraType.VIEWPORT)
            {
                m_IsRenderRequired |= CurrentCamera.transform.hasChanged;
                CurrentCamera.transform.hasChanged = false;
            }

            if(m_IsRenderRequired == false) return;

            StartRendering().Forget();
        }

        private async UniTask StartRendering()
        {
            if(IsRendering) await CancelRender();
            m_CancellationTokenSource?.Dispose();
            m_CancellationTokenSource = new ();
            CancellationToken cancellationToken = m_CancellationTokenSource.Token;
            IsRendering = true;
            m_IsRenderRequired = false;
            try
            {
                if(m_IsTileBasedRenderingEnabled.Value == false)
                {
                    for(uint currentSample = 0; currentSample < m_NumSamples.Value; ++currentSample)
                    {
                        m_Renderer.SetShaderParameters(CurrentCamera, m_Resolution.Value, currentSample);
                        m_Renderer.StartRendering(m_ResultTexture);
                        OnRenderProgress?.Invoke();
                        await UniTask.Yield(cancellationToken);
                    }
                }
                else
                {
                    for(uint currentSample = 0; currentSample < m_NumSamples.Value; ++currentSample)
                    {
                        m_Renderer.SetShaderParameters(CurrentCamera, m_Resolution.Value, currentSample);
                        await m_Renderer.StartRendering(m_ResultTexture, m_TileSize.Value, cancellationToken, _ => OnRenderProgress?.Invoke());
                        cancellationToken.ThrowIfCancellationRequested();
                    }
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

        public void SwitchCamera(CameraType cameraType)
        {
            foreach(var (type, camera) in m_Cameras)
            {
                camera.enabled = type == cameraType;
            }
            CurrentCameraType = cameraType;
            RequireRendering();
        }

        public void SetEnabled(bool enable)
        {
            enabled = enable;
            CurrentCamera.enabled = enable;
        }
                
        public void RequireRendering()
        {
            m_IsRenderRequired = true;
        }

        public async UniTask CancelRender()
        {
            m_CancellationTokenSource.Cancel();
            await UniTask.WaitUntil(() => m_Renderer.IsRendering == false);
        }
    }
}