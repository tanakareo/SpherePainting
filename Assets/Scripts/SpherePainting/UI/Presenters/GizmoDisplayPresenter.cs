using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpherePainting
{
    [RequireComponent(typeof(UIDocument))]
    public class GizmoDisplayPresenter : MonoBehaviour
    {
        [SerializeField] private GizmoDisplay m_GizmoDisplay;
        private void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            var gizmoDisplayDropdown = root.Q<GizmoDropdownToggle>("gizmo-dropdown-toggle");
            gizmoDisplayDropdown.RegisterCallbackOnce<ChangeEvent<bool>>(evt =>
            {
                Init();
            });

            var gizmoToggle = root.Q<Toggle>("gizmo-display-toggle");
            gizmoToggle.SetValueWithoutNotify(m_GizmoDisplay.IsActive.CurrentValue);
            m_GizmoDisplay.IsActive.Subscribe(v =>
            {
                gizmoToggle.SetValueWithoutNotify(v);
            }).AddTo(this);
            gizmoToggle.RegisterValueChangedCallback(evt => 
            {
                m_GizmoDisplay.SetActive(evt.newValue);
            });
        }

        private void Init()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            var gizmoDropdownContentsContainer = root.parent.Q<VisualElement>(className : "dropdown__contents-container");
            m_GizmoDisplay.IsActive.Subscribe(v =>
            {
                gizmoDropdownContentsContainer.SetEnabled(v);
            }).AddTo(this);
            var renderingCameraGizmoStrengthSlider = root.parent.Q<GizmoStrengthSlider>("rendering-camera-gizmo-strength-slider");
            var generationBoundsGizmoStrengthSlider = root.parent.Q<GizmoStrengthSlider>("generation-bounds-gizmo-strength-slider");
            var gridGizmoStrengthSlider = root.parent.Q<GizmoStrengthSlider>("grid-gizmo-strength-slider");
            var canvasMaskGizmoStrengthSlider = root.parent.Q<GizmoStrengthSlider>("canvas-mask-gizmo-strength-slider");
            Dictionary<GizmoType, GizmoStrengthSlider> gizmoStrengthSliders = new () {
                {GizmoType.RENDERING_CAMERA, renderingCameraGizmoStrengthSlider},
                {GizmoType.GRID, gridGizmoStrengthSlider},
                {GizmoType.GENERATION_BOUNDS, generationBoundsGizmoStrengthSlider},
                {GizmoType.CANVAS_MASK, canvasMaskGizmoStrengthSlider},
            };
            foreach(var (gizmoType, gizmoSlider) in gizmoStrengthSliders)
            {
                Gizmo gizmo = m_GizmoDisplay.Gizmos[gizmoType];
                gizmoSlider.SetValueWithoutNotify(gizmo.Opacity.CurrentValue);
                gizmoSlider.SetToggleValueWithoutNotify(gizmo.IsActive.CurrentValue);
                gizmoSlider.RegisterValueChangedCallback(evt =>
                {
                    gizmo.SetOpacity(evt.newValue);
                });
                gizmoSlider.OnActiveToggleChanged += evt =>
                {
                    gizmo.SetActive(evt.newValue);
                };
                gizmo.IsActive.Subscribe(v =>
                {
                    gizmoSlider.SetToggleValueWithoutNotify(v);
                }).AddTo(this);
                gizmo.Opacity.Subscribe(v =>
                {
                    gizmoSlider.SetValueWithoutNotify(v);
                }).AddTo(this);
            }
        }
    }
}