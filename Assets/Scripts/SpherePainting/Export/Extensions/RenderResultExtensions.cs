using System.IO;

namespace SpherePainting
{
    public static class RenderResultExtensions
    {
        public static string[] ExportAsPNGs(this RenderResult result, string folderPath, string fileNameBase)
        {
            string[] imagePaths = new string[result.LayerCount];
            for(int i = 0; i < result.LayerCount; ++i)
            {
                string fileName = $"{fileNameBase}_{i:00}";
                imagePaths[i] = Path.Combine(folderPath, $"{fileName}.png");
                result.RenderTextures[i].ExportAsPNG(folderPath, fileName);
            }
            return imagePaths;
        }
    }
}