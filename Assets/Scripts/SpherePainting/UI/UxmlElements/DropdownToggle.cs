using UnityEngine.UIElements;

namespace SpherePainting
{
    [UxmlElement("DropdownToggle")]
    public partial class DropdownToggle : Toggle
    {
        private readonly VisualElement m_Background;
        private readonly VisualElement m_ContentsContainer;
        private bool m_IsPointerLeft = false;

        public DropdownToggle()
        {
            this.RegisterValueChangedCallback(evt =>
            {
                if(evt.newValue) ShowContents();
                else HideContents();
            });
            
            m_Background = new VisualElement()
            {
                style = {
                    position = Position.Absolute,
                }
            };

            m_Background.RegisterCallback<PointerDownEvent>(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                value = false;
            });
            m_ContentsContainer = new VisualElement()
            {
                style = {
                    position = Position.Absolute
                }
            };
            
            m_ContentsContainer.AddToClassList("dropdown__contents-container");
            m_ContentsContainer.RegisterCallback<PointerLeaveEvent>(evt =>
            {
                m_IsPointerLeft = true;
                VisualElement capturingElement = (VisualElement)panel.GetCapturingElement(evt.pointerId);

                if(m_ContentsContainer.Contains(capturingElement))return;
                
                value = false;
            });
            m_ContentsContainer.RegisterCallback<PointerCaptureOutEvent>(evt =>
            {
                if(m_IsPointerLeft == false) return;

                value = false;
            });
            var tail = new VisualElement()
            {
                pickingMode = PickingMode.Ignore,
            };
            tail.AddToClassList("dropdown__contents-container__tail");
            m_ContentsContainer.Add(tail);
            float safeAreaWidth = 20.0f;
            var pointerClickSafeArea = new VisualElement()
            {
                style = {
                    position = Position.Absolute,
                    left = -safeAreaWidth,
                    right = -safeAreaWidth,
                    bottom = -safeAreaWidth
                }
            };
            m_ContentsContainer.Add(pointerClickSafeArea);

            m_Background.Add(m_ContentsContainer);
            RegisterCallback<GeometryChangedEvent>(evt =>
            {
                m_Background.style.width = panel.visualTree.resolvedStyle.width;
                m_Background.style.height = panel.visualTree.resolvedStyle.height;
            });
            m_Background.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                pointerClickSafeArea.style.top = -tail.resolvedStyle.height * 0.5f;
                m_ContentsContainer.style.left = worldBound.position.x - (m_ContentsContainer.resolvedStyle.width - resolvedStyle.width) * 0.5f;
                m_ContentsContainer.style.top =  worldBound.position.y + resolvedStyle.height + tail.resolvedStyle.borderBottomWidth * 0.5f;
            });
        }

        private void ShowContents()
        {
            m_IsPointerLeft = false;
            panel.visualTree.Add(m_Background);
        }

        private void HideContents()
        {
            m_Background.RemoveFromHierarchy();
        }

        public void AddItem(VisualElement item)
        {
            m_ContentsContainer.Add(item);
        }
    }
}