using System;
using UnityEngine.UIElements;

namespace SpherePainting
{
    // 値の編集が終了したときのイベントを登録するManipulator
    public class EndEditManipulator<TValue> : Manipulator
    {
        private event Action OnEndEdit;
        private readonly INotifyValueChanged<TValue> m_TargetField;
        private bool m_IsValueChanged = false;

        public EndEditManipulator(INotifyValueChanged<TValue> field, Action onEndEdit)
        {
            m_TargetField = field;
            OnEndEdit = onEndEdit;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            if (m_TargetField is VisualElement visualElement)
            {
                visualElement.RegisterCallback<MouseCaptureOutEvent>(OnEndEditEvent);
                visualElement.RegisterCallback<FocusOutEvent>(OnEndEditEvent);
                m_TargetField.RegisterValueChangedCallback(OnValueChanged);
            }
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            if (m_TargetField is VisualElement visualElement)
            {
                visualElement.UnregisterCallback<MouseCaptureOutEvent>(OnEndEditEvent);
                visualElement.UnregisterCallback<FocusOutEvent>(OnEndEditEvent);
                m_TargetField.UnregisterValueChangedCallback(OnValueChanged);
            }
        }

        private void OnValueChanged(ChangeEvent<TValue> evt)
        {
            m_IsValueChanged = true;
        }

        private void OnEndEditEvent(EventBase evt)
        {
            if(m_IsValueChanged == false) return;
            m_IsValueChanged = false;
            OnEndEdit?.Invoke();
        }
    }
}
