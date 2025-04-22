using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpherePainting
{
    [RequireComponent(typeof(UIDocument))]
    public class SphereRenderSettingPresenter : MonoBehaviour
    {
        [SerializeField] private SphereRenderSetting m_SphereRenderSetting;

        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            var displaySphereBackFaceToggle = root.Q<Toggle>("display-sphere-back-face-toggle");
            m_SphereRenderSetting.DisplayBackFace.Subscribe(v =>
            {
                displaySphereBackFaceToggle.SetValueWithoutNotify(v);
            }).AddTo(this);
            displaySphereBackFaceToggle.RegisterValueChangedCallback(evt =>
            {
                m_SphereRenderSetting.SetDisplayBackFace(evt.newValue);
            });

            var sphereBlendStrengthSlider = root.Q<Slider>("sphere-blend-strength-slider");
            var operationTargetBlendStrengthSlider = root.Q<Slider>("operation-target-blend-strength-slider");
            var sphereMaterialBlendStrengthSlider = root.Q<Slider>("sphere-material-blend-strength-slider");
            var operationTargetMaterialBlendStrengthSlider = root.Q<Slider>("operation-target-material-blend-strength-slider");
            var operationSmoothnessSlider = root.Q<Slider>("operation-smoothness-slider");
            var operationMaterialSmoothnessSlider = root.Q<Slider>("operation-material-smoothness-slider");

            // スライダーの最小値、最大値を設定
            sphereBlendStrengthSlider.lowValue = 0.0f;
            sphereBlendStrengthSlider.highValue = 2.0f;
            operationTargetBlendStrengthSlider.lowValue = 0.0f;
            operationTargetBlendStrengthSlider.highValue = 2.0f;
            sphereMaterialBlendStrengthSlider.lowValue = 0.0f;
            sphereMaterialBlendStrengthSlider.highValue = 1.0f;
            operationTargetMaterialBlendStrengthSlider.lowValue = 0.0f;
            operationTargetMaterialBlendStrengthSlider.highValue = 1.0f;
            operationSmoothnessSlider.lowValue = 0.0f;
            operationSmoothnessSlider.highValue = 1.0f;
            operationMaterialSmoothnessSlider.lowValue = 0.0f;
            operationMaterialSmoothnessSlider.highValue = 1.0f;

            // 球のレンダリングの設定を監視して、スライダーに反映
            m_SphereRenderSetting.SphereBlendStrength.Subscribe(v => sphereBlendStrengthSlider.SetValueWithoutNotify(v)).AddTo(this);
            m_SphereRenderSetting.OperationTargetBlendStrength.Subscribe(v => operationTargetBlendStrengthSlider.SetValueWithoutNotify(v)).AddTo(this);
            m_SphereRenderSetting.SphereMaterialBlendStrength.Subscribe(v => sphereMaterialBlendStrengthSlider.SetValueWithoutNotify(v)).AddTo(this);
            m_SphereRenderSetting.OperationTargetMaterialBlendStrength.Subscribe(v => operationTargetMaterialBlendStrengthSlider.SetValueWithoutNotify(v)).AddTo(this);
            m_SphereRenderSetting.OperationSmoothness.Subscribe(v => operationSmoothnessSlider.SetValueWithoutNotify(v)).AddTo(this);
            m_SphereRenderSetting.OperationMaterialSmoothness.Subscribe(v => operationMaterialSmoothnessSlider.SetValueWithoutNotify(v)).AddTo(this);

            // スライダーの値が変更されたら、球のレンダリングの設定を更新
            sphereBlendStrengthSlider.RegisterValueChangedCallback(v => 
            {
                if(v.target != v.currentTarget) return;
                m_SphereRenderSetting.SetSphereBlendStrength(v.newValue);
            });
            operationTargetBlendStrengthSlider.RegisterValueChangedCallback(v =>
            {
                if(v.target != v.currentTarget) return;
                m_SphereRenderSetting.SetOperationTargetBlendStrength(v.newValue);
            });
            sphereMaterialBlendStrengthSlider.RegisterValueChangedCallback(v =>
            {
                if(v.target != v.currentTarget) return;
                m_SphereRenderSetting.SetSphereMaterialBlendStrength(v.newValue);
            });
            operationTargetMaterialBlendStrengthSlider.RegisterValueChangedCallback(v =>
            {
                if(v.target != v.currentTarget) return;
                m_SphereRenderSetting.SetOperationTargetMaterialBlendStrength(v.newValue);
            });
            operationSmoothnessSlider.RegisterValueChangedCallback(v =>
            {
                if(v.target != v.currentTarget) return;
                m_SphereRenderSetting.SetOperationSmoothness(v.newValue);
            });
            operationMaterialSmoothnessSlider.RegisterValueChangedCallback(v =>
            {
                if(v.target != v.currentTarget) return;
                m_SphereRenderSetting.SetOperationMaterialSmoothness(v.newValue);
            });
        }
    }
}
