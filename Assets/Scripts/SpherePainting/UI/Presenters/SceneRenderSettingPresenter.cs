using System.Collections.Generic;
using R3;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpherePainting
{
    public class SceneRenderSettingPresenter : MonoBehaviour
    {        
        private static readonly int SLIDER_COLOR_RANGE_MAX = 255;

        [SerializeField] private SceneRenderSetting m_SceneRenderSetting;
        private Toggle m_SpatialDistortionToggle;
        private List<VisualElement> m_SpatialDistortionElements = new(10);

        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            
            var backgroundColorHueSlider = root.Q<SliderInt>("background-color-hue-slider-int");
            var backgroundColorSaturationSlider = root.Q<SliderInt>("background-color-saturation-slider-int");
            var backgroundColorValueSlider = root.Q<SliderInt>("background-color-value-slider-int");
            var backgroundColorOpacitySlider = root.Q<SliderInt>("background-color-opacity-slider-int");

            var displayBackgroundToggle = root.Q<Toggle>("display-background-toggle");

            backgroundColorHueSlider.value = (int)(m_SceneRenderSetting.BackgroundHSVColor.Value.x * SLIDER_COLOR_RANGE_MAX);
            backgroundColorSaturationSlider.value = (int)(m_SceneRenderSetting.BackgroundHSVColor.Value.y * SLIDER_COLOR_RANGE_MAX);
            backgroundColorValueSlider.value = (int)(m_SceneRenderSetting.BackgroundHSVColor.Value.z * SLIDER_COLOR_RANGE_MAX);
            backgroundColorOpacitySlider.value = (int)(m_SceneRenderSetting.BackgroundHSVColor.Value.w * SLIDER_COLOR_RANGE_MAX);
            displayBackgroundToggle.value = m_SceneRenderSetting.DisplayBackground.Value;

            backgroundColorHueSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                float4 color = m_SceneRenderSetting.BackgroundHSVColor.Value;
                color.x = (float)evt.newValue / SLIDER_COLOR_RANGE_MAX;
                m_SceneRenderSetting.BackgroundHSVColor.Value = color;
            });
            backgroundColorSaturationSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                float4 color = m_SceneRenderSetting.BackgroundHSVColor.Value;
                color.y = (float)evt.newValue / SLIDER_COLOR_RANGE_MAX;
                m_SceneRenderSetting.BackgroundHSVColor.Value = color;
            });
            backgroundColorValueSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                float4 color = m_SceneRenderSetting.BackgroundHSVColor.Value;
                color.z = (float)evt.newValue / SLIDER_COLOR_RANGE_MAX;
                m_SceneRenderSetting.BackgroundHSVColor.Value = color;
            });
            backgroundColorOpacitySlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                float4 color = m_SceneRenderSetting.BackgroundHSVColor.Value;
                color.w = (float)evt.newValue / SLIDER_COLOR_RANGE_MAX;
                m_SceneRenderSetting.BackgroundHSVColor.Value = color;
            });
            m_SceneRenderSetting.BackgroundHSVColor.Subscribe(v =>
            {
                backgroundColorHueSlider.SetValueWithoutNotify((int)(v.x * SLIDER_COLOR_RANGE_MAX));
                backgroundColorSaturationSlider.SetValueWithoutNotify((int)(v.y * SLIDER_COLOR_RANGE_MAX));
                backgroundColorValueSlider.SetValueWithoutNotify((int)(v.z * SLIDER_COLOR_RANGE_MAX));
                backgroundColorOpacitySlider.SetValueWithoutNotify((int)(v.w * SLIDER_COLOR_RANGE_MAX));
            }).AddTo(this);
            displayBackgroundToggle.RegisterValueChangedCallback(evt =>
            {
                m_SceneRenderSetting.DisplayBackground.Value = evt.newValue;
            });
            m_SceneRenderSetting.DisplayBackground.Subscribe(v =>
            {
                displayBackgroundToggle.SetValueWithoutNotify(v);
            }).AddTo(this);

            m_SpatialDistortionToggle = root.Q<Toggle>("spatial-distortion-toggle");
            var rayRotationAroundZAxisFloatField = root.Q<FloatField>("ray-rotation-around-z-axis-float-field");
            var rayAmplitudeVector2Field = root.Q<Vector2Field>("ray-amplitude-vector2-field");
            var rayFrequencyVector2Field = root.Q<Vector2Field>("ray-frequency-vector2-field");
            var rayPhaseOffsetVector2Field = root.Q<Vector2Field>("ray-phase-offset-vector2-field");
            m_SpatialDistortionElements.Add(rayRotationAroundZAxisFloatField);
            m_SpatialDistortionElements.Add(rayAmplitudeVector2Field);
            m_SpatialDistortionElements.Add(rayFrequencyVector2Field);
            m_SpatialDistortionElements.Add(rayPhaseOffsetVector2Field);
            m_SpatialDistortionToggle.value = m_SceneRenderSetting.IsSpatialDistortionEnabled.CurrentValue;
            UpdateSpatialDistortionElementsDisplay();
            m_SpatialDistortionToggle.RegisterValueChangedCallback(v =>
            {
                m_SceneRenderSetting.SetSpatialDistortionEnabled(v.newValue);
            });
            m_SceneRenderSetting.IsSpatialDistortionEnabled.Subscribe(v =>
            {
                m_SpatialDistortionToggle.SetValueWithoutNotify(v);
                UpdateSpatialDistortionElementsDisplay();
            }).AddTo(this);
            rayPhaseOffsetVector2Field.SetValueWithoutNotify(m_SceneRenderSetting.RayPhaseOffset.CurrentValue);
            rayRotationAroundZAxisFloatField.OnEndEdit(() =>
            {
                m_SceneRenderSetting.SetRayRotationAroundZAxis(rayRotationAroundZAxisFloatField.value);
            });
            rayAmplitudeVector2Field.OnEndEdit(() =>
            {
                m_SceneRenderSetting.SetRayAmplitude(rayAmplitudeVector2Field.value);
            });
            rayFrequencyVector2Field.OnEndEdit(() =>
            {
                m_SceneRenderSetting.SetRayFrequency(rayFrequencyVector2Field.value);
            });
            rayPhaseOffsetVector2Field.OnEndEdit(() =>
            {
                m_SceneRenderSetting.SetRayPhaseOffset(rayPhaseOffsetVector2Field.value);
            });
            m_SceneRenderSetting.RayRotationAroundZAxis.Subscribe(v =>
            {
                rayRotationAroundZAxisFloatField.SetValueWithoutNotify(m_SceneRenderSetting.RayRotationAroundZAxis.CurrentValue);
            }).AddTo(this);
            m_SceneRenderSetting.RayAmplitude.Subscribe(v =>
            {
                rayAmplitudeVector2Field.SetValueWithoutNotify(m_SceneRenderSetting.RayAmplitude.CurrentValue);
            }).AddTo(this);
            m_SceneRenderSetting.RayFrequency.Subscribe(v =>
            {
                rayFrequencyVector2Field.SetValueWithoutNotify(m_SceneRenderSetting.RayFrequency.CurrentValue);
            }).AddTo(this);
            m_SceneRenderSetting.RayPhaseOffset.Subscribe(v =>
            {
                rayPhaseOffsetVector2Field.SetValueWithoutNotify(m_SceneRenderSetting.RayPhaseOffset.CurrentValue);
            }).AddTo(this);
        }

        private void UpdateSpatialDistortionElementsDisplay()
        {
            bool display = m_SpatialDistortionToggle.value;
            foreach(var element in m_SpatialDistortionElements)
            {
                element.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
    }
}
