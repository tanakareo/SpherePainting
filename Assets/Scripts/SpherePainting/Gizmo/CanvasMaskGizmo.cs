using R3;
using UnityEngine;

namespace SpherePainting
{
    public class CanvasMaskGizmo : Gizmo
    {
        [SerializeField] private ViewportRendering m_ViewportRendering;
        [SerializeField] private Canvas m_Canvas;
        [SerializeField] private RenderTexture m_SourceRenderTexture;
        [SerializeField] private RenderTexture m_DestinationRenderTexture;

        private Material m_Material;

        private readonly ReactiveProperty<float> m_Opacity = new (1.0f);
        private readonly ReactiveProperty<bool> m_IsActive = new (false);

        public override ReadOnlyReactiveProperty<float> Opacity => m_Opacity;
        public override ReadOnlyReactiveProperty<bool> IsActive => m_IsActive;

        private bool m_IsUpdateRequired = true;

        private void Awake()
        {
            m_Material = new Material(Shader.Find("Hidden/CanvasMaskShader"));
            m_Canvas.CurrentShapeType.Subscribe(v =>
            {
                bool isEllipse = m_Canvas.CurrentShapeType.CurrentValue == Canvas.ShapeType.ELLIPSE;
                m_Material.SetFloat("_IsEllipse", isEllipse ? 1.0f : 0.0f);
            }).AddTo(this);
            m_Canvas.CurrentTextureScale.Subscribe(v =>
            {
                m_Material.SetVector("_Scale", v);
            }).AddTo(this);
            m_Canvas.CurrentTextureOffset.Subscribe(v =>
            {
                m_Material.SetVector("_Offset", v);
                m_IsUpdateRequired = true;
            }).AddTo(this);
            m_Material.SetFloat("_Strength", m_Opacity.Value);

            m_ViewportRendering.OnRenderProgress += () =>
            {
                m_IsUpdateRequired = true;
            };
        }

        private void Update()
        {
            if (m_IsUpdateRequired == false) return;

            Mask();
        }

        private void Mask()
        {
            if(m_IsActive.Value)
            {
                Graphics.Blit(m_SourceRenderTexture, m_DestinationRenderTexture, m_Material);
            }
            else
            {
                Graphics.Blit(m_SourceRenderTexture, m_DestinationRenderTexture);
            }
            m_IsUpdateRequired = false;
        }

        // 表示するかどうかを設定
        public override void SetActive(bool active)
        {
            m_IsActive.Value = active;
            m_IsUpdateRequired = true;
        }

        public override void SetOpacity(float opacity)
        {
            m_Opacity.Value = opacity;
            m_Material.SetFloat("_Strength", m_Opacity.Value);
            m_IsUpdateRequired = true;
        }
    }
}