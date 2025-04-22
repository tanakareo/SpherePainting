using UnityEngine;
using UnityEngine.UIElements;

namespace SpherePainting
{
    [RequireComponent(typeof(UIDocument))]
    public class ViewportPresenter : MonoBehaviour
    {
        [SerializeField] private ViewportModeController m_ViewportModeController;
        [SerializeField] private ViewportRendering m_ViewportRendering;

        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            var setCameraToRenderingCameraToggle = root.Q<Toggle>("set-camrea-to-rendering-camera-toggle");
            setCameraToRenderingCameraToggle.SetValueWithoutNotify(m_ViewportRendering.CurrentCameraType == CameraType.RENDERING);
            setCameraToRenderingCameraToggle.RegisterValueChangedCallback(evt =>
            {
                if(evt.newValue == true)
                {
                    m_ViewportRendering.SwitchCamera(CameraType.RENDERING);
                }
                else
                {
                    m_ViewportRendering.SwitchCamera(CameraType.VIEWPORT);
                }
            });
            var viewportCameraControlEventReceiver = root.Q<CameraControlEventReceiver>("viewport-camera-control-event-receiver");
            viewportCameraControlEventReceiver.OnDragPointer += (pointerDelta, pressedButtons) =>
            {
                m_ViewportModeController.CurrentMode.Content.OnDragPointer(pointerDelta, pressedButtons);
            };
            viewportCameraControlEventReceiver.OnScroll += scrollDelta =>
            {
                m_ViewportModeController.CurrentMode.Content.OnScroll(scrollDelta);
            };
            viewportCameraControlEventReceiver.OnKeyDown += (keyCode) =>
            {
                m_ViewportModeController.CurrentMode.Content.OnKeyDown(keyCode);
            };
        }
    }
}
