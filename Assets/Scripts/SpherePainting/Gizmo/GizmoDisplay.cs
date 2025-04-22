using UnityEngine;
using AYellowpaper.SerializedCollections;
using R3;

namespace SpherePainting
{
    public class GizmoDisplay : MonoBehaviour
    {
        [SerializeField, SerializedDictionary("Type", "Gizmo")] 
        private SerializedDictionary<GizmoType, Gizmo> m_Gizmos;
        public SerializedDictionary<GizmoType, Gizmo> Gizmos => m_Gizmos;
        private readonly ReactiveProperty<bool> m_IsActive = new (true);
        public ReadOnlyReactiveProperty<bool> IsActive => m_IsActive;

        private void Awake()
        {
            foreach(var gizmo in m_Gizmos.Values)
            {
                gizmo.IsActive.Subscribe(v =>
                {
                    if(v == false) return;
                    m_IsActive.Value = true;
                }).AddTo(this);
            }
        }

        public void SetActive(bool active)
        {
            m_IsActive.Value = active;
            
            foreach(var gizmo in m_Gizmos.Values)
            {
                gizmo.SetActive(active);
            }
        }
    }
}