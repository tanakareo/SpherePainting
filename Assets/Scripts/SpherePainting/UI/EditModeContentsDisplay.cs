using UnityEngine;
using UnityEngine.UIElements;

namespace SpherePainting
{
    // 編集モードのUIの表示を設定するクラス
    [RequireComponent(typeof(UIDocument))]
    public class EditModeContentsDisplay : MonoBehaviour
    {
        private VisualElement m_ViewportCameraContent;
        private VisualElement m_RenderingCameraContent;

        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            var setCameraToRenderingCameraToggle = root.Q<Toggle>("set-camrea-to-rendering-camera-toggle");
            m_ViewportCameraContent = root.Q<VisualElement>("viewport-camera-content");
            m_RenderingCameraContent = root.Q<VisualElement>("rendering-camera-content");
            SetContentsDisplay(setCameraToRenderingCameraToggle.value);
            setCameraToRenderingCameraToggle.RegisterValueChangedCallback(evt =>
            {
                SetContentsDisplay(evt.newValue);
            });
        }

        private void SetContentsDisplay(bool isRenderingCamera)
        {
            if(isRenderingCamera)
            {
                m_ViewportCameraContent.style.display = DisplayStyle.None;
                m_RenderingCameraContent.style.display = DisplayStyle.Flex;
            }
            else
            {
                m_ViewportCameraContent.style.display = DisplayStyle.Flex;
                m_RenderingCameraContent.style.display = DisplayStyle.None;
            }
        }
    }
}
