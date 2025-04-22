using System;
using UnityEngine.UIElements;

namespace SpherePainting
{
    [UxmlElement("GizmoStrengthSlider")]
    public partial class GizmoStrengthSlider : VisualElement
    {
        [UxmlAttribute("Label")] private string m_Label;
        private Toggle m_ActiveToggle;
        private UnityEngine.UIElements.Slider m_Slider;
        public float SliderValue => m_ActiveToggle.value ? m_Slider.value : 0.0f;
        public void RegisterValueChangedCallback(Action<ChangeEvent<float>> action)
        {
            m_Slider.RegisterValueChangedCallback(evt => action?.Invoke(evt));
        }
        public Action<ChangeEvent<bool>> OnActiveToggleChanged;

        public GizmoStrengthSlider()
        {
            RegisterCallbackOnce<AttachToPanelEvent>(evt =>
            {
                var label = new Label(m_Label);
                label.AddToClassList("gizmo-strength-slider__label");
                m_ActiveToggle = new Toggle();
                m_ActiveToggle.AddToClassList("gizmo-strength-slider__toggle");
                m_Slider = new UnityEngine.UIElements.Slider
                {
                    label = string.Empty,
                    showInputField = false,
                    fill = true,
                    lowValue = 0.0f,
                    highValue = 1.0f
                };
                m_Slider.AddToClassList("gizmo-strength-slider__slider");
                var contentsContainer = new VisualElement();
                contentsContainer.AddToClassList("gizmo-strength-slider__contents-container");
                contentsContainer.Add(m_ActiveToggle);
                contentsContainer.Add(m_Slider);
                Add(label);
                Add(contentsContainer);
                AddToClassList("gizmo-strength-slider");
                m_Slider.SetEnabled(m_ActiveToggle.value);
                m_ActiveToggle.RegisterValueChangedCallback(evt =>
                {
                    m_Slider.SetEnabled(m_ActiveToggle.value);
                    OnActiveToggleChanged?.Invoke(evt);
                });
            });
        }

        public void SetToggleValueWithoutNotify(bool active)
        {
            m_ActiveToggle.SetValueWithoutNotify(active);
            m_Slider.SetEnabled(active);
        }

        public void SetValueWithoutNotify(float value)
        {
            m_Slider.SetValueWithoutNotify(value);
        }
    }
}
