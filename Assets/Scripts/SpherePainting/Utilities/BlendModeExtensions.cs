using System.Collections.Generic;

namespace SpherePainting
{
    public static class BlendModeExtensions
    {
        private static readonly Dictionary<BlendMode, string> s_BlendModeToJapanese = new()
        {
            { BlendMode.NORMAL, "標準" },
            { BlendMode.MULTIPLY, "乗算" },
            { BlendMode.OVERLAY, "オーバーレイ" },
            { BlendMode.ADD, "加算" },
            { BlendMode.ADD_GLOW, "加算（発光）" },
            { BlendMode.DIFFERENCE, "差分" },
            { BlendMode.EXCLUSION, "排他" },
            { BlendMode.HUE, "色相" },
            { BlendMode.SATURATION, "彩度" },
            { BlendMode.COLOR, "色" },
            { BlendMode.LUMINOSITY, "輝度" },
        };

        public static string ToJapanese(this BlendMode mode)
        {
            return s_BlendModeToJapanese.TryGetValue(mode, out var name) ? name : "不明";
        }
    }
}