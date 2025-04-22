using System;
using Unity.Mathematics;
using UnityEngine;

namespace SpherePainting
{
    public class CanvasMaterialSetting : MonoBehaviour
    {
        [SerializeField] private Material m_BaseMaterial;
        public float3 BaseHSVColor { get; private set; }

        public event Action<float3> OnBaseColorChanged; // キャンバスの色が変更されたときに呼ぶ

        private void Awake()
        {
            float3 hsvColor;
            Color.RGBToHSV(m_BaseMaterial.color, out hsvColor.x, out hsvColor.y, out hsvColor.z);
            BaseHSVColor = hsvColor;
        }

        //　キャンバスの色を設定
        public void SetBaseMaterialColor(float3 hsvColor)
        {
            BaseHSVColor = hsvColor;
            m_BaseMaterial.color = BaseHSVColor.HSVToRGBColor().ToColor();
            OnBaseColorChanged?.Invoke(hsvColor);
        }

        public void SetBaseMaterialColorHue(float hue)
        {
            SetBaseMaterialColor(new float3(hue, BaseHSVColor.yz));
        }
        public void SetBaseMaterialColorSaturation(float saturation)
        {
            SetBaseMaterialColor(new float3(BaseHSVColor.x, saturation, BaseHSVColor.z));
        }
        public void SetBaseMaterialColorValue(float value)
        {
            SetBaseMaterialColor(new float3(BaseHSVColor.xy, value));
        }
    }
}
