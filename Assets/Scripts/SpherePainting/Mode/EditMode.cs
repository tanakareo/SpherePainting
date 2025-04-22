using CameraController;
using UnityEngine;

namespace SpherePainting
{
    public class EditMode : ViewportMode
    {
        [SerializeField] private CameraMouseOrbit m_CameraMouseOrbit;
        [SerializeField] private ViewportRendering m_ViewportRendering;
        
        public override ViewportModeContent Content { get; protected set; }

        void Awake()
        {
            Content = new EditModeContent(m_CameraMouseOrbit, m_ViewportRendering);
        }

        public override void OnDisableMode()
        {
            m_ViewportRendering.SetEnabled(false);
            m_CameraMouseOrbit.enabled = false;
        }

        public override void OnEnableMode()
        {
            m_ViewportRendering.SetEnabled(true);
            m_CameraMouseOrbit.enabled = true;
        }

    }

    public class EditModeContent : ViewportModeContent
    {
        private CameraMouseOrbit m_CameraMouseOrbit;
        private ViewportRendering m_ViewportRendering;

        public EditModeContent(CameraMouseOrbit cameraMouseOrbit, ViewportRendering viewportRendering)
        {
            m_CameraMouseOrbit = cameraMouseOrbit;
            m_ViewportRendering = viewportRendering;
        }

        public override void OnDragPointer(Vector2 pointerDelta, int pressedButtons)
        {
            if(m_ViewportRendering.CurrentCameraType == CameraType.RENDERING) return;

            if(pressedButtons == 0b001)
            {
                m_CameraMouseOrbit.RotateView(pointerDelta);
            }
            else if(pressedButtons == 0b010)
            {
                m_CameraMouseOrbit.MoveView(pointerDelta);
            }
        }

        public override void OnKeyDown(KeyCode keyCode)
        {
            if(keyCode == KeyCode.Space)
            {
                m_CameraMouseOrbit.ResetCameraTransform();
            }
        }

        public override void OnScroll(Vector3 scrollDelta)
        {
            if(m_ViewportRendering.CurrentCameraType == CameraType.RENDERING) return;
            m_CameraMouseOrbit.ZoomView(scrollDelta);
        }
    }
}
