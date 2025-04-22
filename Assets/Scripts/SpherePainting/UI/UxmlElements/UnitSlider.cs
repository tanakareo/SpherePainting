using System.Globalization;
using UnityEngine.UIElements;

namespace SpherePainting
{
    [UxmlElement("UnitSlider")]
    public partial class UnitSlider : UnityEngine.UIElements.Slider
    {
        [UxmlAttribute("UnitSuffix")]
        private string m_UnitSuffix = "unit";
        public string UnitSuffix
        {
            get => m_UnitSuffix;
            set
            {
                m_UnitSuffix = value;
                UpdateTextField();
            }
        }
        
        [UxmlAttribute("UnitConversionFactor")]
        private float m_UnitToUnityUnit = 1.0f;
        // Unity内部単位を変換するスケール
        public float UnitToUnityUnit
        {
            get => m_UnitToUnityUnit;
            set
            {
                m_UnitToUnityUnit = value;
                UpdateTextField();
            }
        }

        private TextField m_TextField;

        private bool m_IsTextFieldFocused;

        public UnitSlider()
        {
            m_TextField = new TextField();
            m_TextField.AddToClassList("unity-base-slider__text-field");
            Add(m_TextField);

            m_TextField.RegisterCallback<FocusInEvent>(evt => m_IsTextFieldFocused = true);
            m_TextField.RegisterCallback<FocusOutEvent>(evt =>
            {
                float newValue;
                if(float.TryParse(m_TextField.value, NumberStyles.Number, CultureInfo.InvariantCulture, out newValue))
                {
                    value = newValue * m_UnitToUnityUnit;
                }
                m_IsTextFieldFocused = false;
                UpdateTextField();
            });

            this.RegisterValueChangedCallback(evt => 
            {
                if(m_IsTextFieldFocused) return;
                UpdateTextField();
            });

            UpdateTextField();
        }

        private void UpdateTextField()
        {
            // スライダー値を指定の単位に変換して表示
            float convertedValue = 1.0f / m_UnitToUnityUnit * value;
            m_TextField.SetValueWithoutNotify($"{convertedValue:F1} {m_UnitSuffix}");
        }

        public override void SetValueWithoutNotify(float newValue)
        {
            base.SetValueWithoutNotify(newValue);
            UpdateTextField();
        }
    }
}
