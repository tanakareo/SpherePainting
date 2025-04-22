using UnityEngine;
using UnityEngine.UIElements;

namespace SpherePainting
{
    [RequireComponent(typeof(UIDocument))]
    public class RenderingCameraSettingPresenter : MonoBehaviour
    {
        [SerializeField] private RenderingCameraSetting m_RenderingCameraSetting;

        private Toggle m_IsOrthographicToggle;
        private UnitSlider m_FieldOfViewSlider;
        private Slider m_OrthographicSizeSlider;

        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            // カメラの位置と回転
            var renderingCameraPositionVector3Field = root.Q<Vector3Field>("rendering-camera-position-vector3-field");
            renderingCameraPositionVector3Field.value = m_RenderingCameraSetting.CameraPosition;
            var renderingCameraRotationVector3Field = root.Q<Vector3Field>("rendering-camera-rotation-vector3-field");
            renderingCameraRotationVector3Field.value = m_RenderingCameraSetting.CameraRotation;
            var setCameraTransformToViewportCameraButton = root.Q<Button>("set-camera-transform-to-viewport-camera-button");
            setCameraTransformToViewportCameraButton.clicked += () =>
            {
                m_RenderingCameraSetting.SetCameraTransformToViewportCamera();
            };
            var resetCameraTransformButton = root.Q<Button>("reset-camera-transform-button");
            resetCameraTransformButton.clicked += () =>
            {
                m_RenderingCameraSetting.ResetCameraTransform();
            };
            renderingCameraPositionVector3Field.OnEndEdit(() =>
            {
                m_RenderingCameraSetting.SetCameraPosition(renderingCameraPositionVector3Field.value);
            });
            renderingCameraRotationVector3Field.OnEndEdit(() =>
            {
                m_RenderingCameraSetting.SetCameraRotation(renderingCameraRotationVector3Field.value);
            });
            m_RenderingCameraSetting.OnCameraTransformChanged += () =>
            {
                renderingCameraPositionVector3Field.SetValueWithoutNotify(m_RenderingCameraSetting.CameraPosition);
                renderingCameraRotationVector3Field.SetValueWithoutNotify(m_RenderingCameraSetting.CameraRotation);
            };

            m_IsOrthographicToggle = root.Q<Toggle>("is-orthographic-toggle");
            m_FieldOfViewSlider = root.Q<UnitSlider>("field-of-view-unit-slider");
            m_OrthographicSizeSlider = root.Q<Slider>("orthographic-size-slider");

            m_IsOrthographicToggle.value = m_RenderingCameraSetting.IsOrthographic;
            UpdateSlidersDisplay();
            m_IsOrthographicToggle.RegisterValueChangedCallback(v => 
            {
                m_RenderingCameraSetting.SetOrthographicFlag(v.newValue);
                UpdateSlidersDisplay();
            });
            m_FieldOfViewSlider.UnitSuffix = "°";
            m_FieldOfViewSlider.UnitToUnityUnit = 1.0f;
            m_FieldOfViewSlider.lowValue = RenderingCameraSetting.MIN_FIELD_OF_VIEW;
            m_FieldOfViewSlider.highValue = RenderingCameraSetting.MAX_FIELD_OF_VIEW;
            m_FieldOfViewSlider.value = m_RenderingCameraSetting.FieldOfView;
            m_FieldOfViewSlider.RegisterValueChangedCallback(v =>
            {
                if(v.target != v.currentTarget) return;
                m_RenderingCameraSetting.SetFieldOfView(v.newValue);
            });

            m_OrthographicSizeSlider.lowValue = RenderingCameraSetting.MIN_ORTHOGRAPHIC_SIZE;
            m_OrthographicSizeSlider.highValue = RenderingCameraSetting.MAX_ORTHOGRAPHIC_SIZE;
            m_OrthographicSizeSlider.value = m_RenderingCameraSetting.OrthographicSize;
            m_OrthographicSizeSlider.RegisterValueChangedCallback(v =>
            {
                if(v.target != v.currentTarget) return;
                m_RenderingCameraSetting.SetOrthographicSize(v.newValue);
            });

            m_RenderingCameraSetting.OnCameraSettingChanged += () =>
            {
                m_IsOrthographicToggle.SetValueWithoutNotify(m_RenderingCameraSetting.IsOrthographic);
                m_FieldOfViewSlider.SetValueWithoutNotify(m_RenderingCameraSetting.FieldOfView);
                m_OrthographicSizeSlider.SetValueWithoutNotify(m_RenderingCameraSetting.OrthographicSize);
                UpdateSlidersDisplay();
            };
        }

        // スライダーの表示を設定
        private void UpdateSlidersDisplay()
        {
            if(m_IsOrthographicToggle.value)
            {
                m_FieldOfViewSlider.style.display = DisplayStyle.None;
                m_OrthographicSizeSlider.style.display = DisplayStyle.Flex;
            }
            else
            {
                m_FieldOfViewSlider.style.display = DisplayStyle.Flex;
                m_OrthographicSizeSlider.style.display = DisplayStyle.None;
            }
        }
    }
}
