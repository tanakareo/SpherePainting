using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpherePainting
{
    [RequireComponent(typeof(UIDocument))]
    public class CanvasMaterialSettingPresenter : MonoBehaviour
    {
        private static readonly int SLIDER_COLOR_RANGE_MAX = 255;

        [SerializeField] private CanvasMaterialSetting m_CanvasMaterialSetting;
        private SliderInt m_BaseColorHueSliderInt, m_BaseColorSaturationSliderInt, m_BaseColorValueSliderInt;

        private void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            // 質感
            m_BaseColorHueSliderInt = root.Q<SliderInt>("canvas-base-color-hue-slider-int");
            m_BaseColorSaturationSliderInt = root.Q<SliderInt>("canvas-base-color-saturation-slider-int");
            m_BaseColorValueSliderInt = root.Q<SliderInt>("canvas-base-color-value-slider-int");
            UpdateBaseColorSliderValues(m_CanvasMaterialSetting.BaseHSVColor);
            m_BaseColorHueSliderInt.RegisterValueChangedCallback(v =>
            {
                if(v.target != v.currentTarget) return;
                m_CanvasMaterialSetting.SetBaseMaterialColorHue((float)v.newValue / SLIDER_COLOR_RANGE_MAX);
            });
            m_BaseColorSaturationSliderInt.RegisterValueChangedCallback(v =>
            {
                if(v.target != v.currentTarget) return;
                m_CanvasMaterialSetting.SetBaseMaterialColorSaturation((float)v.newValue / SLIDER_COLOR_RANGE_MAX);
            });
            m_BaseColorValueSliderInt.RegisterValueChangedCallback(v =>
            {
                if(v.target != v.currentTarget) return;
                m_CanvasMaterialSetting.SetBaseMaterialColorValue((float)v.newValue / SLIDER_COLOR_RANGE_MAX);
            });
            m_CanvasMaterialSetting.OnBaseColorChanged += color => UpdateBaseColorSliderValues(color);
        }

        private void UpdateBaseColorSliderValues(float3 color)
        {
            int3 sliderColor = (int3)(color * SLIDER_COLOR_RANGE_MAX);
            m_BaseColorHueSliderInt.SetValueWithoutNotify(sliderColor.x);
            m_BaseColorSaturationSliderInt.SetValueWithoutNotify(sliderColor.y);
            m_BaseColorValueSliderInt.SetValueWithoutNotify(sliderColor.z);
        }
    }
}