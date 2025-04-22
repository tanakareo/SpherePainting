using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpherePainting
{
    public sealed class DragManipulator : PointerManipulator
    {
        public event Action<Vector2, int> OnDragPointer;
        
        public DragManipulator(VisualElement target)
        {
            // 操作対象のVisualElementがあればtargetに入れる
            this.target = target;

            // マウスの左ボタンだけ反応するようにManipulatorActivationFilterを追加する
            activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });

            activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.RightMouse
            });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            // 各種ポインターイベントを登録
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }

        // ポインター押下時の処理
        private void OnPointerDown(PointerDownEvent evt)
        {
            // コンストラクタで登録したactivatorsにイベントがマッチすれば（今回の場合左クリックだったら）処理を通す
            if (!CanStartManipulation(evt))
                return;

            target.CapturePointer(evt.pointerId);
        }

        // ポインター移動時の処理
        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (!target.HasPointerCapture(evt.pointerId))
                return;
            OnDragPointer?.Invoke(evt.deltaPosition, evt.pressedButtons);
        }

        // ポインターを離した時の処理
        private void OnPointerUp(PointerUpEvent evt)
        {
            if (!CanStopManipulation(evt))
                return;

            if (target.HasPointerCapture(evt.pointerId))
                target.ReleasePointer(evt.pointerId);
        }
    }
}
