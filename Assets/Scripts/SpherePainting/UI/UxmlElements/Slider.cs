using UnityEngine.UIElements;

namespace SpherePainting
{
    [UxmlElement("Slider")]
    public partial class Slider : UnityEngine.UIElements.Slider
    {
        private FloatField m_FloatField;

        public Slider()
        {    
            showInputField = false;
            m_FloatField = new FloatField();
            m_FloatField.value = value;
            Add(m_FloatField);
            m_FloatField.RegisterCallback<FocusOutEvent>(v => value = m_FloatField.value);
            RegisterCallback<AttachToPanelEvent>(e =>
            {
                this.RegisterValueChangedCallback(v => m_FloatField.SetValueWithoutNotify(v.newValue));
            });
        }

        public override void SetValueWithoutNotify(float newValue)
        {
            base.SetValueWithoutNotify(newValue);
            m_FloatField.value = newValue;
        }
    }
}
