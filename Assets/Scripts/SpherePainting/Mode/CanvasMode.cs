using CameraController;
using UnityEngine;

namespace SpherePainting
{
    public class CanvasMode : ViewportMode
    {
        [SerializeField] private Camera m_Camera;
        [SerializeField] private CameraMouseOrbit m_CameraMouseOrbit;

        public override ViewportModeContent Content { get; protected set; }

        private void Awake()
        {
            Content = new CanvasModeContent(m_CameraMouseOrbit);
        }

        public override void OnDisableMode()
        {
            m_Camera.enabled = false;
            m_CameraMouseOrbit.enabled = false;
        }

        public override void OnEnableMode()
        {
            m_Camera.enabled = true;
            m_CameraMouseOrbit.enabled = true;
        }
    }

    public class CanvasModeContent : ViewportModeContent
    {
        private CameraMouseOrbit m_CameraMouseOrbit;

        public CanvasModeContent(CameraMouseOrbit cameraMouseOrbit)
        {
            m_CameraMouseOrbit = cameraMouseOrbit;
        }

        public override void OnDragPointer(Vector2 pointerDelta, int pressedButtons)
        {
            if(pressedButtons != 0b001) return;

            m_CameraMouseOrbit.RotateView(pointerDelta);
        }

        public override void OnKeyDown(KeyCode keyCode) { }

        public override void OnScroll(Vector3 scrollDelta)
        {
            m_CameraMouseOrbit.ZoomView(scrollDelta);
        }
    }
}
