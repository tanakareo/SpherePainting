using UnityEngine.UIElements;

namespace SpherePainting
{
    [UxmlElement("SliderInt")]
    public partial class SliderInt : UnityEngine.UIElements.SliderInt
    {
        private IntegerField m_IntegerField;

        public SliderInt()
        {    
            showInputField = false;
            m_IntegerField = new IntegerField();
            Add(m_IntegerField);
            m_IntegerField.RegisterCallback<FocusOutEvent>(v => value = m_IntegerField.value);
            RegisterCallback<AttachToPanelEvent>(e =>
            {
                this.RegisterValueChangedCallback(v => m_IntegerField.SetValueWithoutNotify(v.newValue));
            });
        }
        
        public override void SetValueWithoutNotify(int newValue)
        {
            base.SetValueWithoutNotify(newValue);
            m_IntegerField.value = newValue;
        }
    }
}
