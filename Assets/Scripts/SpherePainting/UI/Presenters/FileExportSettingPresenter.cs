using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpherePainting
{
    [RequireComponent(typeof(UIDocument))]
    public class FileExportSettingPresenter : MonoBehaviour
    {
        [SerializeField] private FileExporter m_FileExporter;
        [SerializeField] private SphereVolumeExporter m_SphereVolumeExporter;
        private RenderingProgressPopup m_RenderingProgressPopup;
        private CancellationTokenSource m_CancellationTokenSource;

        void Awake()
        {
            m_RenderingProgressPopup = new RenderingProgressPopup();
            m_RenderingProgressPopup.RegisterCancelButtonCallback(() => m_CancellationTokenSource.Cancel());
        }

        private void OnDestroy()
        {
            m_CancellationTokenSource?.Dispose();
        }

        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            var setExportFolderPathButton = root.Q<Button>("set-export-folder-path-button");
            setExportFolderPathButton.clickable.clicked += () =>
            {
                setExportFolderPathButton.Blur();
                m_FileExporter.SetExportFolderPath();
            };
            var exportFolderPathTextField = root.Q<TextField>("export-folder-path-text-field");
            m_FileExporter.ExportFolderPath.Subscribe(v =>
            {
                exportFolderPathTextField.SetValueWithoutNotify(v);
            }).AddTo(this);

            var exportRenderResultButton = root.Q<Button>("export-render-result-button");
            exportRenderResultButton.clickable.clicked += () =>
            {
                exportRenderResultButton.Blur();
                m_FileExporter.ExportRenderResult();
                NativeDialog.ShowMessage("エクスポートが完了しました。");
            };
            m_FileExporter.CanExportRenderResult.Subscribe(v =>
            {
                exportRenderResultButton.SetEnabled(v);
            }).AddTo(this);
            
            var renderAndExportSphereVolumeButton = root.Q<Button>("render-and-export-sphere-volume-button");
            renderAndExportSphereVolumeButton.clickable.clicked += () =>
            {
                m_CancellationTokenSource?.Dispose();
                m_CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(new []{new CancellationTokenSource().Token, destroyCancellationToken});
                m_SphereVolumeExporter.StartRenderingAndExport((progress, sphereIndex, sphereCount) =>
                {
                    m_RenderingProgressPopup.UpdateProgressBar(progress);
                    m_RenderingProgressPopup.UpdateInfoLabel($"{progress * 100.0f:#0.0} %（{sphereIndex} / {sphereCount}）");

                    if(progress < 1.0f) return;

                    m_RenderingProgressPopup.UpdateInfoLabel($"レンダリング終了");
                    m_RenderingProgressPopup.Hide();
                },
                m_CancellationTokenSource.Token).Forget();
                m_RenderingProgressPopup.Show(root.parent);
            };

            var createSVGFileToggle = root.Q<Toggle>("should-create-svg-file-toggle");
            createSVGFileToggle.value = m_FileExporter.ShouldCreateSVGFile.CurrentValue;
            var createShuffledLayersOnlyToggle = root.Q<Toggle>("should-create-only-shuffled-layers-toggle");
            createShuffledLayersOnlyToggle.value = m_FileExporter.ShouldCreateOnlyShuffledLayers.CurrentValue;
            createShuffledLayersOnlyToggle.style.display = m_FileExporter.ShouldCreateSVGFile.CurrentValue ? DisplayStyle.Flex : DisplayStyle.None;
            m_FileExporter.ShouldCreateSVGFile.Subscribe(v =>
            {
                createShuffledLayersOnlyToggle.style.display = v ? DisplayStyle.Flex : DisplayStyle.None;
                createSVGFileToggle.SetValueWithoutNotify(v);
            }).AddTo(this);
            m_FileExporter.ShouldCreateOnlyShuffledLayers.Subscribe(v =>
            {
                createShuffledLayersOnlyToggle.SetValueWithoutNotify(v);
            }).AddTo(this);
            createSVGFileToggle.RegisterValueChangedCallback(evt =>
            {
                m_FileExporter.SetShouldCreateSVGFile(evt.newValue);
            });
            createShuffledLayersOnlyToggle.RegisterValueChangedCallback(evt =>
            {
                m_FileExporter.SetShouldCreateOnlyShuffledLayers(evt.newValue);
            });
            var autoExportAfterRenderingToggle = root.Q<Toggle>("should-auto-export-after-rendering-toggle");
            autoExportAfterRenderingToggle.RegisterValueChangedCallback(evt =>
            {
                m_FileExporter.SetShouldAutoExportAfterRendering(evt.newValue);
            });
            m_FileExporter.ShouldAutoExportAfterRendering.Subscribe(v =>
            {
                autoExportAfterRenderingToggle.SetValueWithoutNotify(v);
            }).AddTo(this);

            var exportViewportPreviewButton = root.Q<Button>("export-viewport-preview-button");
            var exportCanvasPreviewButton = root.Q<Button>("export-canvas-preview-button");
            exportViewportPreviewButton.clickable.clicked += () =>
            {
                exportViewportPreviewButton.Blur();
                m_FileExporter.ExportRenderTexture(ExportableRenderTextureType.VIEWPORT);
                NativeDialog.ShowMessage("エクスポートが完了しました。");
            };
            exportCanvasPreviewButton.clickable.clicked += () =>
            {
                exportCanvasPreviewButton.Blur();
                m_FileExporter.ExportRenderTexture(ExportableRenderTextureType.CANVAS);
                NativeDialog.ShowMessage("エクスポートが完了しました。");
            };
        }
    }
}
