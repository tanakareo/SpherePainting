using System;
using UnityEngine;

namespace SpherePainting
{
    public class RenderingCameraSetting : MonoBehaviour
    {
        // 最小値と最大値
        public static readonly float MIN_ORTHOGRAPHIC_SIZE = -100.0f;
        public static readonly float MAX_ORTHOGRAPHIC_SIZE = 100.0f;
        public static readonly float MIN_FIELD_OF_VIEW = 0.01f;
        public static readonly float MAX_FIELD_OF_VIEW = 179f;

        [SerializeField] private Camera m_Camera;
        [SerializeField] private Transform m_ViewportRenderingCameraTransform;
        [SerializeField] private ViewportRendering m_ViewportRendering;

        public bool IsOrthographic => m_Camera.orthographic;
        public float OrthographicSize => m_Camera.orthographicSize;
        public float FieldOfView => m_Camera.fieldOfView;

        public event Action OnCameraSettingChanged; // カメラの設定が変更時に呼ぶ
        public event Action OnCameraTransformChanged; // カメラのトランスフォームが変更時に呼ぶ

        public Vector3 CameraPosition => m_Camera.transform.position;
        public Vector3 CameraRotation => m_Camera.transform.rotation.eulerAngles;
        private Vector3 m_InitialCameraPosition;
        private Quaternion m_InitialCameraRotation;

        void Awake()
        {
            m_InitialCameraPosition = m_Camera.transform.position;
            m_InitialCameraRotation = m_Camera.transform.rotation;
            OnCameraSettingChanged += () => m_ViewportRendering.RequireRendering();
        }

        public void SetOrthographicFlag(bool isOrthographic)
        {
            m_Camera.orthographic = isOrthographic;
            OnCameraSettingChanged?.Invoke();
        }

        public void SetOrthographicSize(float orthographicSize)
        {
            m_Camera.orthographicSize = orthographicSize;
            OnCameraSettingChanged?.Invoke();
        }

        public void SetFieldOfView(float fieldOfView)
        {
            m_Camera.fieldOfView = fieldOfView;
            OnCameraSettingChanged?.Invoke();
        }

        public void SetCameraPosition(Vector3 position)
        {
            m_Camera.transform.position = position;
            OnCameraSettingChanged?.Invoke();
            OnCameraTransformChanged?.Invoke();
        }
        
        public void SetCameraRotation(Vector3 rotation)
        {
            m_Camera.transform.rotation = Quaternion.Euler(rotation);
            OnCameraSettingChanged?.Invoke();
            OnCameraTransformChanged?.Invoke();
        }

        public void ResetCameraTransform()
        {
            m_Camera.transform.position = m_InitialCameraPosition;
            m_Camera.transform.rotation = m_InitialCameraRotation;
            OnCameraSettingChanged?.Invoke();
            OnCameraTransformChanged?.Invoke();
        }

        public void SetCameraTransformToViewportCamera()
        {
            m_Camera.transform.SetPositionAndRotation(m_ViewportRenderingCameraTransform.position, m_ViewportRenderingCameraTransform.rotation);
            OnCameraSettingChanged?.Invoke();
            OnCameraTransformChanged?.Invoke();
        }
    }
}
