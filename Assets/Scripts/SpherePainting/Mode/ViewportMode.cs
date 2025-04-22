using UnityEngine;

namespace SpherePainting
{
    [System.Serializable]
    public abstract class ViewportMode : MonoBehaviour
    {
        public abstract void OnEnableMode();
        public abstract void OnDisableMode();
        public abstract ViewportModeContent Content { get; protected set; }
    }
}
