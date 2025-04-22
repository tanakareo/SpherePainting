using UnityEngine;
using R3;
using SFB;
using AYellowpaper.SerializedCollections;
using System;

namespace SpherePainting
{
    public class FileExporter : MonoBehaviour
    {
        [SerializeField] private CanvasSVGExporter m_CanvasSVGExporter;
        [SerializeField] private RenderResult m_RenderResult;
        public ReadOnlyReactiveProperty<bool> CanExportRenderResult => m_RenderResult.IsEmpty.Select(v => !v).ToReadOnlyReactiveProperty();
        [SerializeField, SerializedDictionary("Type", "RenderTexture")] private SerializedDictionary<ExportableRenderTextureType, ExportableRenderTexture> m_ExportableRenderTextures;
        private readonly ReactiveProperty<string> m_ExportFolderPath = new("");
        public ReadOnlyReactiveProperty<string> ExportFolderPath => m_ExportFolderPath;
        private readonly ReactiveProperty<bool> m_ShouldCreateSVGFile = new (true);
        public ReadOnlyReactiveProperty<bool> ShouldCreateSVGFile => m_ShouldCreateSVGFile;
        public void SetShouldCreateSVGFile(bool value) => m_ShouldCreateSVGFile.Value = value;
        private readonly ReactiveProperty<bool> m_ShouldCreateOnlyShuffledLayers = new (false);
        public ReadOnlyReactiveProperty<bool> ShouldCreateOnlyShuffledLayers => m_ShouldCreateOnlyShuffledLayers;
        public void SetShouldCreateOnlyShuffledLayers(bool value) => m_ShouldCreateOnlyShuffledLayers.Value = value;
        private readonly ReactiveProperty<bool> m_ShouldAutoExportAfterRendering = new (false);
        public ReadOnlyReactiveProperty<bool> ShouldAutoExportAfterRendering => m_ShouldAutoExportAfterRendering;
        public void SetShouldAutoExportAfterRendering(bool value) => m_ShouldAutoExportAfterRendering.Value = value;
        [SerializeField] private FinalRendering m_FinalRendering;

        private void Awake()
        {
            InitExportFolderPath();
            m_FinalRendering.OnRenderComplete += () =>
            {
                if (m_ShouldAutoExportAfterRendering.Value)
                {
                    ExportRenderResult();
                }
            };
        }

        private void InitExportFolderPath() => m_ExportFolderPath.Value = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public void SetExportFolderPath()
        {
            string[] folderPaths = StandaloneFileBrowser.OpenFolderPanel("", "", false);
            if (folderPaths.Length == 0 || string.IsNullOrEmpty(folderPaths[0])) return;
            m_ExportFolderPath.Value = folderPaths[0];
        }

        // レンダリング結果をエクスポート
        public void ExportRenderResult()
        {
            if (m_RenderResult.IsEmpty.CurrentValue) return;
            string[] exportedImagePaths = m_RenderResult.ExportAsPNGs(m_ExportFolderPath.Value, $"Layer");
            if (!m_ShouldCreateSVGFile.Value) return;
            m_CanvasSVGExporter.Export(exportedImagePaths, m_ExportFolderPath.Value, "RenderResult", m_ShouldCreateOnlyShuffledLayers.Value);
        }

        public void ExportRenderTexture(ExportableRenderTextureType type)
        {
            if(m_ExportableRenderTextures.TryGetValue(type, out var exportableRenderTexture) == false)
            {
                Debug.LogWarning("ExportableRenderTexture not found.");
                return;
            }

            exportableRenderTexture.ExportAsPNG(m_ExportFolderPath.Value);
        }
    }
}
