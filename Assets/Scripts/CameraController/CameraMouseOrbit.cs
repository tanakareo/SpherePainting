using UnityEngine;

namespace CameraController
{
    public class CameraMouseOrbit : MonoBehaviour
    {
        [SerializeField] private Transform m_TargetCameraTransform;

        [SerializeField] private Vector3 m_CurrentLookTargetPosition = Vector3.zero;
        [SerializeField] private Vector3 m_DesiredLookTargetPosition = Vector3.zero;
        [SerializeField] private float m_Damping = 0.9995f;
        
        [Header("Move")]
        [SerializeField] private float m_MoveSensitivity = 12.0f;

        [Header("Rotation")]
        [SerializeField] private Vector2 m_XAngleRange = new Vector2(-85.0f, 85.0f);
        [SerializeField] private bool m_ClampYAngle = false;
        [SerializeField] private Vector2 m_YAngleRange = new Vector2(0f, 360.0f);
        [SerializeField] private float m_RotationSensitivity = 12.0f;

        [Header("Distance")]
        [SerializeField] private float m_CurrentDistance = 20.0f;
        [SerializeField] private Vector2 m_DistanceRange = new Vector2(2.0f, 10.0f);
        [SerializeField] private float m_DistanceSensitivity = 12.0f;

        // Rotation
        private Vector2 m_DesiredRotation = Vector2.zero;
        [SerializeField] private Vector2 m_CurrentRotation = Vector2.zero;

        // Distance
        private float m_DesiredDistance = 0.0f;

        private float m_InitialDistance;
        private Vector2 m_InitialRotation;
        private Vector3 m_InitialLookTargetPosition;

        void Awake()
        {
            m_InitialRotation = m_CurrentRotation;
            m_InitialDistance = m_CurrentDistance;
            m_InitialLookTargetPosition = m_CurrentLookTargetPosition;

            m_DesiredRotation = m_CurrentRotation;
            m_DesiredDistance = m_CurrentDistance;
            m_DesiredLookTargetPosition = m_CurrentLookTargetPosition;
        }

        void Update()
        {
            UpdateCamera();
        }

        public void MoveView(Vector2 mouseDelta)
        {
            Vector3 right = m_TargetCameraTransform.right;
            Vector3 up = m_TargetCameraTransform.up;
            Vector3 move = (right * -mouseDelta.x + up * mouseDelta.y) * m_MoveSensitivity;
            m_DesiredLookTargetPosition += move;
        }

        public void RotateView(Vector2 mouseDelta)
        {
            Vector2 deltaRotation = new Vector2(mouseDelta.y, mouseDelta.x) * m_RotationSensitivity;
            m_DesiredRotation += deltaRotation;
            m_DesiredRotation.x = Mathf.Clamp(m_DesiredRotation.x, m_XAngleRange.x, m_XAngleRange.y);
            if (m_ClampYAngle)
            {
                m_DesiredRotation.y = Mathf.Clamp(m_DesiredRotation.y, m_YAngleRange.x, m_YAngleRange.y);
            }
        }

        public void ZoomView(Vector2 scrollDelta)
        {
            m_DesiredDistance += scrollDelta.y * m_DistanceSensitivity;
            m_DesiredDistance = Mathf.Clamp(m_DesiredDistance, m_DistanceRange.x, m_DistanceRange.y);
        }

        private void UpdateCamera()
        {
            // Smooth rotation and distance
            m_CurrentDistance = FixedLerpSmoothing(m_CurrentDistance, m_DesiredDistance, m_Damping);
            m_CurrentRotation = FixedLerpSmoothing(m_CurrentRotation, m_DesiredRotation, m_Damping);
            m_CurrentLookTargetPosition = FixedLerpSmoothing(m_CurrentLookTargetPosition, m_DesiredLookTargetPosition, m_Damping);

            // Calculate camera position
            Vector3 targetOffset = Quaternion.Euler(m_CurrentRotation) * -Vector3.forward * m_CurrentDistance;
            m_TargetCameraTransform.position = m_CurrentLookTargetPosition + targetOffset;
            m_TargetCameraTransform.LookAt(m_CurrentLookTargetPosition);
        }

        public void ResetCameraTransform()
        {
            m_DesiredDistance = m_InitialDistance;
            m_DesiredRotation = m_InitialRotation;
            m_DesiredLookTargetPosition = m_InitialLookTargetPosition;
        }

        private float FixedLerpSmoothing(float a, float b, float F)
        {
            float f1 = (1 - F) * a + F * b;
            float r = float.MaxValue;
            if (a != b) r = (f1 - b) / (a - b);
            return Mathf.Lerp(a, b, 1 - Mathf.Pow(r, Time.deltaTime));
        }

        private Vector2 FixedLerpSmoothing(Vector2 a, Vector2 b, float F)
        {
            return new Vector2(FixedLerpSmoothing(a.x, b.x, F), FixedLerpSmoothing(a.y, b.y, F));
        }

        private Vector3 FixedLerpSmoothing(Vector3 a, Vector3 b, float F)
        {
            return new Vector3(FixedLerpSmoothing(a.x, b.x, F), FixedLerpSmoothing(a.y, b.y, F), FixedLerpSmoothing(a.z, b.z, F));
        }
    }
}
