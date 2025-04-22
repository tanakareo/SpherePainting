using R3;
using UnityEngine;

namespace SpherePainting
{
    public abstract class Gizmo : MonoBehaviour
    {
        public abstract ReadOnlyReactiveProperty<float> Opacity { get; }
        public abstract ReadOnlyReactiveProperty<bool> IsActive { get; }

        public abstract void SetActive(bool active);
        public abstract void SetOpacity(float strength);
    }
}