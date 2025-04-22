using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SpherePainting
{
    public static  class GizmoDisplayDataHandler
    {
        [Serializable]
        public class GizmoDisplayData
        {
            [JsonProperty] public Dictionary<GizmoType, float> GizmoOpacity { get; private set; }
            [JsonProperty] public Dictionary<GizmoType, bool> IsGizmoActive { get; private set; }


            public GizmoDisplayData(){}

            public GizmoDisplayData(GizmoDisplay gizmoDisplay)
            {
                GizmoOpacity = gizmoDisplay.Gizmos.ToDictionary(pair => pair.Key, pair => pair.Value.Opacity.CurrentValue);
                IsGizmoActive = gizmoDisplay.Gizmos.ToDictionary(pair => pair.Key, pair => pair.Value.IsActive.CurrentValue);
            }
        }

        public static GizmoDisplayData ToData(this GizmoDisplay gizmoDisplay)
        {
            GizmoDisplayData data = new (gizmoDisplay);
            return data;
        }

        public static void SetData(this GizmoDisplay gizmoDisplay, GizmoDisplayData gizmoDisplayData)
        {
            foreach(var (gizmoType, opacity) in gizmoDisplayData.GizmoOpacity)
            {
                if(gizmoDisplay.Gizmos.TryGetValue(gizmoType, out Gizmo gizmo))
                {
                    gizmo.SetOpacity(opacity);
                }
            }
            foreach(var (gizmoType, isActive) in gizmoDisplayData.IsGizmoActive)
            {
                if(gizmoDisplay.Gizmos.TryGetValue(gizmoType, out Gizmo gizmo))
                {
                    gizmo.SetActive(isActive);
                }
            }
        }
    }
}
