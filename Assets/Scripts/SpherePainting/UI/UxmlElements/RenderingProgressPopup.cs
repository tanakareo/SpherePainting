using System;
#if UNITY_EDITOR
using UnityEditor;
#else
using UnityEngine;
#endif
using UnityEngine.UIElements;
using LitMotion;
using LitMotion.Extensions;

namespace SpherePainting
{
    [UxmlElement("RenderingProgressPopup")]
    public partial class RenderingProgressPopup : VisualElement
    {
        private VisualElement m_Background;
        private VisualElement m_Popup;
        private ProgressBar m_ProgressBar;
        private Label m_InfoLabel;
        private Button m_CancelButton;

        public RenderingProgressPopup()
        {
            #if UNITY_EDITOR
            var treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Resources/UI/RenderingProgressPopup.uxml");
            #else
            var treeAsset = Resources.Load<VisualTreeAsset>("UI/RenderingProgressPopup");
            #endif
            
            var container = treeAsset.Instantiate();
            hierarchy.Add(container);

            m_Background = this.Q<VisualElement>("popup-base");
            m_Popup = this.Q<VisualElement>("rendering-progress-popup");
            m_ProgressBar = this.Q<ProgressBar>("rendering-progress-bar");
            m_InfoLabel = this.Q<Label>("rendering-progress-info-label");
            m_CancelButton = this.Q<Button>("cancel-render-button");
            RegisterCallback<GeometryChangedEvent>(evt =>
            {
                if (panel != null)
                {
                    m_Background.style.width = panel.visualTree.resolvedStyle.width;
                    m_Background.style.height = panel.visualTree.resolvedStyle.height;
                }
            });
        }
        
        public void Show(VisualElement parent)
        {
            parent.Add(this);
            RegisterCallbackOnce<GeometryChangedEvent>(evt =>
            {
                m_Popup.style.bottom = m_Popup.resolvedStyle.width * -0.5f;
                m_Popup.style.left = (resolvedStyle.width - m_Popup.resolvedStyle.width) / 2;
                LMotion.Create(m_Popup.resolvedStyle.width * -0.5f, 20f, 0.4f)
                        .WithEase(Ease.OutCirc)
                        .BindToStyleBottom(m_Popup);
            });
        }

        public void Hide()
        {
            LMotion.Create(20f, m_Popup.resolvedStyle.width * -0.5f, 0.5f)
                    .WithEase(Ease.InBack)
                    .WithDelay(0.1f)
                    .WithOnComplete(RemoveFromHierarchy)
                    .BindToStyleBottom(m_Popup);
        }

        public void UpdateProgressBar(float progress)
        {
            m_ProgressBar.value = progress;
        }

        // 情報を表示するラベルを更新
        public void UpdateInfoLabel(string info)
        {
            m_InfoLabel.text = info;
        }

        public void RegisterCancelButtonCallback(Action action)
        {
            m_CancelButton.clickable.clicked += action;
        }
    }
}
