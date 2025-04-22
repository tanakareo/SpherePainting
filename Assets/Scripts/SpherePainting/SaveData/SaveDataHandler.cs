using System;
using System.IO;
using System.Text;
using SFB;
using Newtonsoft.Json;
using UnityEngine;

namespace SpherePainting
{
    public class SaveDataHandler : MonoBehaviour
    {
        private static readonly string s_ProjectsDirectory = Application.dataPath + "/Projects/";
        private static readonly string s_SystemProjectsDirectory = s_ProjectsDirectory + "/SystemProjects/";
        private static readonly string s_DefaultProjectPath = s_SystemProjectsDirectory + "default.json";
        private static readonly string s_PreviousProjectPath = s_SystemProjectsDirectory + "previous.json";
        private static readonly string s_AutoSaveDirectory = s_ProjectsDirectory + "AutoSaves/";

        [SerializeField] private SphereDataCreator m_SphereDataCreator;
        [SerializeField] private SphereMaterialDataCreator m_SphereMaterialDataCreator;
        [SerializeField] private SceneRenderSetting m_SceneRenderSetting;
        [SerializeField] private SphereRenderSetting m_SphereRenderSetting;
        [SerializeField] private RenderingCameraSetting m_RenderingCameraSetting;
        [SerializeField] private FinalRendering m_FinalRendering;
        [SerializeField] private ViewportRendering m_ViewportRendering;
        [SerializeField] private Canvas m_Canvas;
        [SerializeField] private CanvasMaterialSetting m_CanvasMaterialSetting;
        [SerializeField] private GizmoDisplay m_GizmoDisplay;

        public bool IsAutoSaveEnabled { get; private set; } = true;
        public void SetIsAutoSaveEnabled(bool value) { IsAutoSaveEnabled = value; }
        public float AutoSaveInterval { get; private set; } = 5; // オートセーブの間隔時間（分）
        public void SetAutoSaveInterval(float value) { AutoSaveInterval = value; }
        public int MaxAutoSaveCount { get; private set; } = 20; // 保存データのキャッシュ数
        public void SetMaxAutoSaveCount(int value) { MaxAutoSaveCount = value; }

        private float m_LastAutoSaveTime;

        private void Awake()
        {
            if (!Directory.Exists(s_ProjectsDirectory))
                Directory.CreateDirectory(s_ProjectsDirectory);
            if (!Directory.Exists(s_SystemProjectsDirectory))
                Directory.CreateDirectory(s_SystemProjectsDirectory);
            if (!Directory.Exists(s_AutoSaveDirectory))
                Directory.CreateDirectory(s_AutoSaveDirectory);

            m_LastAutoSaveTime = Time.time;
        }

        private void Start()
        {
            if(!File.Exists(s_DefaultProjectPath))
            {
                Debug.Log("Default project file not found. Creating a new one.");
                Save(s_DefaultProjectPath);
            }
        }

        private void Update()
        {
            if(!IsAutoSaveEnabled) return;
            if (Time.time - m_LastAutoSaveTime >= AutoSaveInterval * 60.0f)
            {
                AutoSave();
                m_LastAutoSaveTime = Time.time;
            }
        }

        public void Save()
        {
            string filePath = StandaloneFileBrowser.SaveFilePanel("", s_ProjectsDirectory, "", "json");
            Save(filePath);
        }

        private void Save(string filePath)
        {
            if(string.IsNullOrEmpty(filePath)) return;

            SaveData data = new
            (
                m_SphereDataCreator.ToData(),
                m_SphereMaterialDataCreator.ToData(),
                m_SceneRenderSetting.ToData(),
                m_SphereRenderSetting.ToData(),
                m_RenderingCameraSetting.ToData(),
                m_FinalRendering.ToData(),
                m_ViewportRendering.ToData(),
                m_Canvas.ToData(),
                m_CanvasMaterialSetting.ToData(),
                m_GizmoDisplay.ToData()
            );
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };
            string json = JsonConvert.SerializeObject(data, settings);
            File.WriteAllText(filePath, json);
        }
        
        private void AutoSave()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string autoSaveFilePath = Path.Combine(s_AutoSaveDirectory, $"autosave_{timestamp}.json");
            Save(autoSaveFilePath);
            ManageAutoSaveFiles();
        }

        private void ManageAutoSaveFiles()
        {
            var autoSaveFiles = new DirectoryInfo(s_AutoSaveDirectory).GetFiles("autosave_*.json");
            if (autoSaveFiles.Length <= MaxAutoSaveCount) return;
            Array.Sort(autoSaveFiles, (x, y) => x.CreationTime.CompareTo(y.CreationTime));
            for (int i = 0; i < autoSaveFiles.Length - MaxAutoSaveCount; i++)
            {
                autoSaveFiles[i].Delete();
            }
        }

        public void Load()
        {
            string[] filePaths = StandaloneFileBrowser.OpenFilePanel("", s_ProjectsDirectory, "json", false);
            if (filePaths.Length == 0) return;
            Load(filePaths[0]);
        }


        private void Load(string filePath)
        {
            if(string.IsNullOrEmpty(filePath)) return;

            try
            {
                string json = File.ReadAllText(filePath, Encoding.UTF8);
                SaveData data = JsonConvert.DeserializeObject<SaveData>(json);
                
                if (data == null)
                {
                    Debug.LogError("Failed to load: JSON deserialization returned null.");
                    return;
                }

                m_SphereDataCreator.SetData(data.SphereDataCreatorData);
                m_SphereMaterialDataCreator.SetData(data.SphereMaterialDataCreatorData);
                m_SceneRenderSetting.SetData(data.SceneRenderSettingData);
                m_SphereRenderSetting.SetData(data.SphereRenderSettingData);
                m_RenderingCameraSetting.SetData(data.RenderingCameraSettingData);
                m_FinalRendering.SetData(data.FinalRenderingData);
                m_ViewportRendering.SetData(data.ViewportRenderingData);
                m_Canvas.SetData(data.CanvasData);
                m_CanvasMaterialSetting.SetData(data.CanvasMaterialSettingData);
                m_GizmoDisplay.SetData(data.GizmoDisplayData);
            }
            catch(Exception ex)
            {
                Debug.LogError($"Failed to load JSON file: {ex.Message}");
            }
        }

        public void LoadNewData()
        {
            Load(s_DefaultProjectPath);
        }

        public bool ExitsPreviousData()
        {
            return File.Exists(s_PreviousProjectPath);
        }

        public void LoadPreviousData()
        {
            if(!ExitsPreviousData()) return;
            Load(s_PreviousProjectPath);
        }

        private void OnApplicationQuit()
        {
            Save(s_PreviousProjectPath);
        }
    }
}
