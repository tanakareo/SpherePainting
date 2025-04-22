using UnityEngine;

namespace SpherePainting
{
    [System.Serializable]
    public abstract class ViewportModeContent
    {
        public abstract void OnDragPointer(Vector2 pointerDelta, int pressedButtons);
        public abstract void OnScroll(Vector3 scrollDelta);
        public abstract void OnKeyDown(KeyCode keyCode);
    }
}
