using Newtonsoft.Json;

namespace SpherePainting
{
    using SphereDataCreatorData = SphereDataCreatorDataHandler.SphereDataCreatorData;
    using SphereMaterialDataCreatorData = SphereMaterialDataCreatorDataHandler.SphereMaterialDataCreatorData;
    using SceneRenderSettingData = SceneRenderSettingDataHandler.SceneRenderSettingData;
    using SphereRenderSettingData = SphereRenderSettingDataHandler.SphereRenderSettingData;
    using RenderingCameraSettingData = RenderingCameraSettingDataHandler.RenderingCameraSettingData;
    using FinalRenderingData = FinalRenderingDataHandler.FinalRenderingData;
    using ViewportRenderingData = ViewportRenderingDataHandler.ViewportRenderingData;
    using CanvasData = CanvasDataHandler.CanvasData;
    using CanvasMaterialSettingData = CanvasMaterialSettingDataHandler.CanvasMaterialSettingData;
    using GizmoDisplayData = GizmoDisplayDataHandler.GizmoDisplayData;

    public class SaveData
    {
        [JsonProperty] public SphereDataCreatorData SphereDataCreatorData { get; private set; }
        [JsonProperty] public SphereMaterialDataCreatorData SphereMaterialDataCreatorData { get; private set; }
        [JsonProperty] public SceneRenderSettingData SceneRenderSettingData { get; private set; }
        [JsonProperty] public SphereRenderSettingData SphereRenderSettingData { get; private set; }
        [JsonProperty] public RenderingCameraSettingData RenderingCameraSettingData { get; private set; }
        [JsonProperty] public FinalRenderingData FinalRenderingData { get; private set; }
        [JsonProperty] public ViewportRenderingData ViewportRenderingData { get; private set; }
        [JsonProperty] public CanvasData CanvasData { get; private set; }
        [JsonProperty] public CanvasMaterialSettingData CanvasMaterialSettingData { get; private set; }
        [JsonProperty] public GizmoDisplayData GizmoDisplayData { get; private set; }
        
        public SaveData(SphereDataCreatorData sphereDataCreatorData,
                        SphereMaterialDataCreatorData sphereMaterialDataCreatorData,
                        SceneRenderSettingData sceneRenderSettingData,
                        SphereRenderSettingData sphereRenderSettingData,
                        RenderingCameraSettingData renderingCameraSettingData,
                        FinalRenderingData finalRenderingData,
                        ViewportRenderingData viewportRenderingData,
                        CanvasData canvasData,
                        CanvasMaterialSettingData canvasMaterialSettingData,
                        GizmoDisplayData gizmoDisplayData)
        {
            SphereDataCreatorData = sphereDataCreatorData;
            SphereMaterialDataCreatorData = sphereMaterialDataCreatorData;
            SceneRenderSettingData = sceneRenderSettingData;
            SphereRenderSettingData = sphereRenderSettingData;
            RenderingCameraSettingData = renderingCameraSettingData;
            FinalRenderingData = finalRenderingData;
            ViewportRenderingData = viewportRenderingData;
            CanvasData = canvasData;
            CanvasMaterialSettingData = canvasMaterialSettingData;
            GizmoDisplayData = gizmoDisplayData;
        }
    }
}