using System;
using UnityEngine.UIElements;

namespace SpherePainting
{
    public static class UIElementsExtensions
    {
        // 値の編集が終了したときに呼び出される
        public static void OnEndEdit<TValue>(this INotifyValueChanged<TValue> field, Action onEndEdit)
        {
            if (field is not VisualElement visualElement) return;
            
            visualElement.AddManipulator(new EndEditManipulator<TValue>(field, onEndEdit));
        }
    }
}
