using UnityEngine;
using UnityEngine.UIElements;

namespace SpherePainting
{
    public static class ToggleButtonGroupExtension
    {
        public static void SetContentsDisplay(this ToggleButtonGroup toggleButtonGroup, params VisualElement[] element)
        {
            var value = toggleButtonGroup.value;
            for(int i = 0; i < value.length; ++i)
            {
                element[i].style.display = value[i] ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
    }
}
