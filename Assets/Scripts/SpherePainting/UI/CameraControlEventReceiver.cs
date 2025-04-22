using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpherePainting
{
    // カメラ操作の入力を受けイベントを発行するクラス
    [UxmlElement("CameraControlEventReceiver")]
    public partial class CameraControlEventReceiver : VisualElement
    {
        private DragManipulator m_DragManipulator;
        public event Action<Vector2, int> OnDragPointer // ドラッグイベント
        {
            add 
            {
                m_DragManipulator.OnDragPointer += value;
            }
            remove
            {
                m_DragManipulator.OnDragPointer -= value;
            }
        }
        public event Action<Vector3> OnScroll; // スクロールイベント

        public event Action<KeyCode> OnKeyDown; // キーを押したときのイベント

        public CameraControlEventReceiver()
        {
            m_DragManipulator = new DragManipulator(this);
            this.AddManipulator(m_DragManipulator);
            RegisterCallback<WheelEvent>(evt => OnScroll?.Invoke(evt.delta) );
            RegisterCallback<KeyDownEvent>(evt =>
            {
                evt.StopPropagation();
                focusController.IgnoreEvent(evt);
                OnKeyDown?.Invoke(evt.keyCode);
            });
        }
    }
}
