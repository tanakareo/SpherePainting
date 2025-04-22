using UnityEngine;

namespace SpherePainting
{
    [CreateAssetMenu(fileName = "ExportableRenderTexture", menuName = "SpherePainting/ExportableRenderTexture")]
    public class ExportableRenderTexture : ScriptableObject
    {
        [SerializeField] private string m_FileName;
        [SerializeField] private RenderTexture m_RenderTexture;
        public void ExportAsPNG(string folderPath) => m_RenderTexture.ExportAsPNG(folderPath, m_FileName);
    }
}