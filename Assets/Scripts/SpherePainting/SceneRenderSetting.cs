using R3;
using Unity.Mathematics;
using UnityEngine;

namespace SpherePainting
{
    public class SceneRenderSetting : MonoBehaviour
    {
        [SerializeField] private SerializableReactiveProperty<bool> m_DisplayBackground;
        [SerializeField] private SerializableReactiveProperty<float4> m_BackgroundHSVColor = new (new float4(0.0f, 0.0f, 0.0f, 0.0f));
        [SerializeField] private ViewportRendering m_ViewportRendering;

        // 背景を表示するかどうか
        public ReactiveProperty<bool> DisplayBackground => m_DisplayBackground;
        public ReactiveProperty<float4> BackgroundHSVColor => m_BackgroundHSVColor;
        public float4 BackgroundRGBColor => m_BackgroundHSVColor.Value.HSVToRGBColor();

        // 空間の歪みを有効にするかどうか
        private readonly ReactiveProperty<bool> m_IsSpatialDistortionEnabled = new (false);
        public ReadOnlyReactiveProperty<bool> IsSpatialDistortionEnabled => m_IsSpatialDistortionEnabled;
        public void SetSpatialDistortionEnabled(bool enable)
        {
            m_IsSpatialDistortionEnabled.Value = enable;
            m_ViewportRendering.RequireRendering();
        }

        // 空間の歪みに関するプロパティ
        // レイのZ軸の回転
        private readonly ReactiveProperty<float> m_RayRotationAroundZAxis = new (0.0f);
        public ReadOnlyReactiveProperty<float> RayRotationAroundZAxis => m_RayRotationAroundZAxis;
        public void SetRayRotationAroundZAxis(float rotationAroundZAxis)
        {
            m_RayRotationAroundZAxis.Value = rotationAroundZAxis;
            m_ViewportRendering.RequireRendering();
        }
        // レイの回転
        private readonly ReactiveProperty<Vector2> m_RayAmplitude = new (Vector2.zero);
        public ReadOnlyReactiveProperty<Vector2> RayAmplitude => m_RayAmplitude;
        public void SetRayAmplitude(Vector2 rayAmplitude)
        {
            m_RayAmplitude.Value = rayAmplitude;
            m_ViewportRendering.RequireRendering();
        }
        // レイの周波数
        private readonly ReactiveProperty<Vector2> m_RayFrequency = new (Vector2.one);
        public ReadOnlyReactiveProperty<Vector2> RayFrequency => m_RayFrequency;
        public void SetRayFrequency(Vector2 rayFrequency)
        {
            m_RayFrequency.Value = rayFrequency;
            m_ViewportRendering.RequireRendering();
        }
        // レイの位相
        private readonly ReactiveProperty<Vector2> m_RayPhaseOffset = new (Vector2.zero);
        public ReadOnlyReactiveProperty<Vector2> RayPhaseOffset => m_RayPhaseOffset;
        public void SetRayPhaseOffset(Vector2 rayPhaseOffset)
        {
            m_RayPhaseOffset.Value = rayPhaseOffset;
            m_ViewportRendering.RequireRendering();
        }

        void Awake()
        {
            DisplayBackground.Subscribe(v =>
            {
                m_ViewportRendering.RequireRendering();
            }).AddTo(this);

            BackgroundHSVColor.Subscribe(v =>
            {
                m_ViewportRendering.RequireRendering();
            }).AddTo(this);
        }
    }
}
