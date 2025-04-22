using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpherePainting
{
    [RequireComponent(typeof(UIDocument))]
    public class RenderingSettingPresenter : MonoBehaviour
    {
        [SerializeField] private FinalRendering m_FinalRendering;
        [SerializeField] private ViewportRendering m_ViewportRendering;
        private RenderingProgressPopup m_RenderingProgressPopup;
        private CancellationTokenSource m_CancellationTokenSource;

        void Awake()
        {
            m_RenderingProgressPopup = new RenderingProgressPopup();
            m_RenderingProgressPopup.RegisterCancelButtonCallback(() => m_CancellationTokenSource.Cancel());
        }

        private void OnDestroy()
        {
            if(m_CancellationTokenSource == null) return;
            
            m_CancellationTokenSource.Cancel();
            m_CancellationTokenSource.Dispose();
        }

        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            
            // ViewportRendeirng
            var viewportSampleCountSliderInt = root.Q<SliderInt>("viewport-sample-count-slider-int");
            viewportSampleCountSliderInt.value = (int)m_ViewportRendering.NumSamples.CurrentValue;
            viewportSampleCountSliderInt.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_ViewportRendering.SetNumSamples((uint)evt.newValue);
            });
            m_ViewportRendering.NumSamples.Subscribe(v =>
            {
                viewportSampleCountSliderInt.SetValueWithoutNotify((int)v);
            }).AddTo(this);

            var viewportTileSizeVector2IntField = root.Q<Vector2IntField>("viewport-tile-size-vector2-int-field");
            viewportTileSizeVector2IntField.value = m_ViewportRendering.Resolution.CurrentValue;
            m_ViewportRendering.TileSize.Subscribe(v => 
            {
                viewportTileSizeVector2IntField.SetValueWithoutNotify(v);
            }).AddTo(this);
            viewportTileSizeVector2IntField.OnEndEdit(() =>
            {
                Vector2Int tileSize = Vector2Int.Min(Vector2Int.Max(Vector2Int.one * 64, viewportTileSizeVector2IntField.value), m_ViewportRendering.Resolution.CurrentValue);
                viewportTileSizeVector2IntField.SetValueWithoutNotify(tileSize);
                m_ViewportRendering.SetTileSize(tileSize);
            });

            var viewportEnableTileBaseRenderingToggle = root.Q<Toggle>("viewport-enable-tile-base-rendering-toggle");
            viewportEnableTileBaseRenderingToggle.SetValueWithoutNotify(m_ViewportRendering.IsTileBasedRenderingEnabled.CurrentValue);
            viewportTileSizeVector2IntField.style.display = m_ViewportRendering.IsTileBasedRenderingEnabled.CurrentValue ? DisplayStyle.Flex : DisplayStyle.None;
            viewportEnableTileBaseRenderingToggle.RegisterValueChangedCallback(evt =>
            {
                m_ViewportRendering.SetTileBasedRenderingEnabled(evt.newValue);
            });
            m_ViewportRendering.IsTileBasedRenderingEnabled.Subscribe(v =>
            {
                viewportTileSizeVector2IntField.style.display = v ? DisplayStyle.Flex : DisplayStyle.None;
                viewportEnableTileBaseRenderingToggle.SetValueWithoutNotify(v);
            }).AddTo(this);

            var viewportResolutionVector2IntField = root.Q<Vector2IntField>("viewport-resolution-vector2-int-field");
            viewportResolutionVector2IntField.value = m_ViewportRendering.Resolution.CurrentValue;
            m_ViewportRendering.Resolution.Subscribe(v =>
            {
                viewportResolutionVector2IntField.SetValueWithoutNotify(v);
            }).AddTo(this);
            viewportResolutionVector2IntField.OnEndEdit(() =>
            {
                Vector2Int resolution = Vector2Int.Max(viewportResolutionVector2IntField.value, Vector2Int.one * 64);
                m_ViewportRendering.SetTileSize(Vector2Int.Min(m_ViewportRendering.TileSize.CurrentValue, resolution));
                viewportResolutionVector2IntField.SetValueWithoutNotify(resolution);
                m_ViewportRendering.SetResolution(resolution).Forget();
            });

            // FinalRendering
            var renderSampleCountSliderInt = root.Q<SliderInt>("render-sample-count-slider-int");
            renderSampleCountSliderInt.value = (int)m_FinalRendering.NumSamples.CurrentValue;
            renderSampleCountSliderInt.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_FinalRendering.SetNumSamples((uint)evt.newValue);
            });
            m_FinalRendering.NumSamples.Subscribe(v =>
            {
                renderSampleCountSliderInt.SetValueWithoutNotify((int)v);
            }).AddTo(this);

            var renderResolutionVector2IntField = root.Q<Vector2IntField>("render-resolution-vector2-int-field");
            renderResolutionVector2IntField.value = m_FinalRendering.Resolution.CurrentValue;
             m_FinalRendering.Resolution.Subscribe(v => renderResolutionVector2IntField.SetValueWithoutNotify(v)).AddTo(this);
            renderResolutionVector2IntField.OnEndEdit(() =>
            {
                Vector2Int resolution = Vector2Int.Max(renderResolutionVector2IntField.value, Vector2Int.one * 64);
                m_FinalRendering.SetTileSize(Vector2Int.Min(m_FinalRendering.TileSize.CurrentValue, resolution));
                renderResolutionVector2IntField.SetValueWithoutNotify(resolution);
                m_FinalRendering.SetResolution(resolution);
            });

            var renderingButton = root.Q<Button>("rendering-button");

            renderingButton.clickable.clicked += () =>
            {
                m_CancellationTokenSource?.Dispose();
                m_CancellationTokenSource = new ();
                m_FinalRendering.StartRendering((progress, layer, totalLayer) =>
                {
                    m_RenderingProgressPopup.UpdateProgressBar(progress);
                    m_RenderingProgressPopup.UpdateInfoLabel($"{progress * 100.0f:#0.0} %（{layer + 1}層目をレンダリング中）");

                    if(progress < 1.0f) return;

                    m_RenderingProgressPopup.UpdateInfoLabel($"レンダリング終了");
                    m_RenderingProgressPopup.Hide();
                },
                m_CancellationTokenSource.Token).Forget();
                m_RenderingProgressPopup.Show(root.parent);
            };
        }
    }
}
