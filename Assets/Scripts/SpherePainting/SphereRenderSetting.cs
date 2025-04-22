using UnityEngine;
using R3;

namespace SpherePainting
{
    public class SphereRenderSetting : MonoBehaviour
    {
        [SerializeField] private ViewportRendering m_ViewportRendering;
        
        // 裏面を表示するかどうか
        private readonly ReactiveProperty<bool> m_DisplayBackFace = new (false);
        public ReadOnlyReactiveProperty<bool> DisplayBackFace => m_DisplayBackFace;
        public void SetDisplayBackFace(bool display)
        {
            m_DisplayBackFace.Value = display;
            m_ViewportRendering.RequireRendering();
        }

        // 球の形状のブレンドの強さ
        private readonly ReactiveProperty<float> m_SphereBlendStrength = new (0.0f);
        public ReadOnlyReactiveProperty<float> SphereBlendStrength => m_SphereBlendStrength;
        public void SetSphereBlendStrength(float strength)
        {
            m_SphereBlendStrength.Value = strength;
            m_ViewportRendering.RequireRendering();
        }
        
        // 演算対象の球の形状のブレンドの強さ
        private readonly ReactiveProperty<float> m_OperationTargetBlendStrength = new (0.0f);
        public ReadOnlyReactiveProperty<float> OperationTargetBlendStrength => m_OperationTargetBlendStrength;
        public void SetOperationTargetBlendStrength(float strength)
        {
            m_OperationTargetBlendStrength.Value = strength;
            m_ViewportRendering.RequireRendering();
        }

        // 球のマテリアルのブレンドの強さ
        private readonly ReactiveProperty<float> m_SphereMaterialBlendStrength = new (0.0f);
        public ReadOnlyReactiveProperty<float> SphereMaterialBlendStrength => m_SphereMaterialBlendStrength;
        public void SetSphereMaterialBlendStrength(float strength)
        {
            m_SphereMaterialBlendStrength.Value = strength;
            m_ViewportRendering.RequireRendering();
        }

        // 演算対象の球のマテリアルのブレンドの強さ
        private readonly ReactiveProperty<float> m_OperationTargetMaterialBlendStrength = new (0.0f);
        public ReadOnlyReactiveProperty<float> OperationTargetMaterialBlendStrength => m_OperationTargetMaterialBlendStrength;
        public void SetOperationTargetMaterialBlendStrength(float strength)
        {
            m_OperationTargetMaterialBlendStrength.Value = strength;
            m_ViewportRendering.RequireRendering();
        }

        // 球の形状の演算の滑らかさ
        private readonly ReactiveProperty<float> m_OperationSmoothness = new (0.0f);
        public ReadOnlyReactiveProperty<float> OperationSmoothness => m_OperationSmoothness;
        public void SetOperationSmoothness(float operationSmoothness)
        {
            m_OperationSmoothness.Value = operationSmoothness;
            m_ViewportRendering.RequireRendering();
        }

        // 球のマテリアルの演算の滑らかさ
        private readonly ReactiveProperty<float> m_OperationMaterialSmoothness = new (0.0f);
        public ReadOnlyReactiveProperty<float> OperationMaterialSmoothness => m_OperationMaterialSmoothness;
        public void SetOperationMaterialSmoothness(float operationMaterialSmoothness)
        {
            m_OperationMaterialSmoothness.Value = operationMaterialSmoothness;
            m_ViewportRendering.RequireRendering();
        }

        private void Awake()
        {
            Init();
        }

        // 初期化
        private void Init()
        {
            m_SphereBlendStrength.Subscribe(v =>
            {
                m_ViewportRendering.RequireRendering();
            }).AddTo(this);

            m_OperationTargetBlendStrength.Subscribe(v =>
            {
                m_ViewportRendering.RequireRendering();
            }).AddTo(this);

            m_SphereMaterialBlendStrength.Subscribe(v => 
            {
                m_ViewportRendering.RequireRendering();
            }).AddTo(this);

            m_OperationTargetMaterialBlendStrength.Subscribe(v => 
            {
                m_ViewportRendering.RequireRendering();
            }).AddTo(this);

            m_OperationSmoothness.Subscribe(v => 
            {
                m_ViewportRendering.RequireRendering();
            }).AddTo(this);

            m_OperationMaterialSmoothness.Subscribe(v => 
            {
                m_ViewportRendering.RequireRendering();
            }).AddTo(this);
        }
    }
}
