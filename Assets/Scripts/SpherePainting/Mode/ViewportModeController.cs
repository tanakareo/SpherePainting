using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace SpherePainting
{
    public class ViewportModeController : MonoBehaviour
    {
        private readonly ViewportModeType m_InitialModeType = ViewportModeType.EDIT;
        public ViewportModeType InitialModeType => m_InitialModeType;
        private ViewportModeType m_CurrentModeType;
        public ViewportMode CurrentMode => m_Modes[m_CurrentModeType];
        [SerializeField, SerializedDictionary("Type", "Mode")] private SerializedDictionary<ViewportModeType, ViewportMode> m_Modes;

        public Action<ViewportModeType> OnSwitchMode { get; set; }

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            foreach(var (type, mode) in m_Modes)
            {
                if(type == m_InitialModeType)
                {
                    mode.OnEnableMode();
                    m_CurrentModeType = type;
                }
                else
                {
                    mode.OnDisableMode();
                }
            }
        }

        public void SwitchMode(ViewportModeType mode)
        {
            if(m_CurrentModeType == mode) return;
            CurrentMode.OnDisableMode();
            m_Modes[mode].OnEnableMode();
            m_CurrentModeType = mode;
            OnSwitchMode?.Invoke(mode);
        }
    }
}