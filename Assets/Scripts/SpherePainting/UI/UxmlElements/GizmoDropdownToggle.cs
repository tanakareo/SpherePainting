#if UNITY_EDITOR
using UnityEditor;
#else
using UnityEngine;
#endif
using UnityEngine.UIElements;

namespace SpherePainting
{
    [UxmlElement("GizmoDropdownToggle")]
    public partial class GizmoDropdownToggle : DropdownToggle
    {
        public GizmoDropdownToggle()
        {
            #if UNITY_EDITOR
            var treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Resources/UI/GizmoDropdownContents.uxml");
            #else
            var treeAsset = Resources.Load<VisualTreeAsset>("UI/GizmoDropdownContents");
            #endif

            var container = treeAsset.Instantiate();
            AddItem(container);
        }
    }
}
