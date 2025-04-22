using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace SpherePainting
{
    [RequireComponent(typeof(UIDocument))]
    public class ViewportModePresenter : MonoBehaviour
    {
        [SerializeField] private ViewportModeController m_ViewportModeController;

        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            var modeToggleButtonGroup = root.Q<ToggleButtonGroup>("mode-toggle-button-group");
            var editModeContent = root.Q<VisualElement>("edit-mode-content");
            var canvasModeContent = root.Q<VisualElement>("canvas-mode-content");
            var editModeButton = root.Q<Button>("edit-mode-button");
            var canvasModeButton = root.Q<Button>("canvas-mode-button");
            var toggleContainer = root.Q<VisualElement>("toggle-container");

            modeToggleButtonGroup.value = CreateModeToggleButtonGroupState(m_ViewportModeController.InitialModeType);
            m_ViewportModeController.OnSwitchMode += modeType => modeToggleButtonGroup.value = CreateModeToggleButtonGroupState(modeType);
            modeToggleButtonGroup.SetContentsDisplay(editModeContent, canvasModeContent);
            toggleContainer.style.display = modeToggleButtonGroup.value[0] ? DisplayStyle.Flex : DisplayStyle.None;
            modeToggleButtonGroup.RegisterValueChangedCallback(evt => 
            {
                modeToggleButtonGroup.SetContentsDisplay(editModeContent, canvasModeContent);
                toggleContainer.style.display = evt.newValue[0] ? DisplayStyle.Flex : DisplayStyle.None;
            });
            editModeButton.clicked += () => m_ViewportModeController.SwitchMode(ViewportModeType.EDIT);
            canvasModeButton.clicked += () => m_ViewportModeController.SwitchMode(ViewportModeType.CANVAS);
        }

        private ToggleButtonGroupState CreateModeToggleButtonGroupState(ViewportModeType modeType)
        {
            List<bool> options = new List<bool>(){modeType == ViewportModeType.EDIT,
                                                  modeType == ViewportModeType.CANVAS};
            return ToggleButtonGroupState.CreateFromOptions(options);
        }
    }
}
