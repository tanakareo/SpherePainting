using UnityEngine;
using UnityEngine.UIElements;

namespace SpherePainting
{
    [RequireComponent(typeof(UIDocument))]
    public class SaveDataSettingPresenter : MonoBehaviour
    {
        [SerializeField] private SaveDataHandler m_SaveDataHandler;

        private void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            Button loadDataButton = root.Q<Button>("load-data-button");
            loadDataButton.clickable.clicked += () =>
            {
                loadDataButton.Blur();
                m_SaveDataHandler.Load();
            };
            Button loadPreviousDataButton = root.Q<Button>("load-previous-data-button");
            loadPreviousDataButton.SetEnabled(m_SaveDataHandler.ExitsPreviousData());
            loadPreviousDataButton.clickable.clicked += () =>
            {
                loadPreviousDataButton.Blur();
                m_SaveDataHandler.LoadPreviousData();
            };
            Button loadNewDataButton = root.Q<Button>("load-new-data-button");
            loadNewDataButton.clickable.clicked += () =>
            {
                loadNewDataButton.Blur();
                m_SaveDataHandler.LoadNewData();
            };
            Toggle enableAutoSaveToggle = root.Q<Toggle>("enable-auto-save-toggle");
            FloatField autoSaveIntervalFloatField = root.Q<FloatField>("auto-save-interval-unit-float-field");
            IntegerField maxAutoSaveCountIntegerField = root.Q<IntegerField>("max-auto-save-count-integer-field");
            enableAutoSaveToggle.value = m_SaveDataHandler.IsAutoSaveEnabled;
            enableAutoSaveToggle.RegisterValueChangedCallback(evt =>
            {
                m_SaveDataHandler.SetIsAutoSaveEnabled(evt.newValue);
                autoSaveIntervalFloatField.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                maxAutoSaveCountIntegerField.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            });
            autoSaveIntervalFloatField.style.display = m_SaveDataHandler.IsAutoSaveEnabled ? DisplayStyle.Flex : DisplayStyle.None;
            autoSaveIntervalFloatField.value = m_SaveDataHandler.AutoSaveInterval;
            autoSaveIntervalFloatField.OnEndEdit(() =>
            {
                autoSaveIntervalFloatField.SetValueWithoutNotify(Mathf.Clamp(autoSaveIntervalFloatField.value, 1, 60));
                m_SaveDataHandler.SetAutoSaveInterval(autoSaveIntervalFloatField.value);
            });
            maxAutoSaveCountIntegerField.style.display = m_SaveDataHandler.IsAutoSaveEnabled ? DisplayStyle.Flex : DisplayStyle.None;
            maxAutoSaveCountIntegerField.value = m_SaveDataHandler.MaxAutoSaveCount;
            maxAutoSaveCountIntegerField.OnEndEdit(() =>
            {
                maxAutoSaveCountIntegerField.SetValueWithoutNotify(Mathf.Clamp(maxAutoSaveCountIntegerField.value, 1, 100));
                m_SaveDataHandler.SetMaxAutoSaveCount(maxAutoSaveCountIntegerField.value);
            });
            Button saveDataButton = root.Q<Button>("save-data-button");
            saveDataButton.clickable.clicked += () =>
            {
                saveDataButton.Blur();
                m_SaveDataHandler.Save();
            };
        }
    }
}