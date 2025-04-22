
using UnityEngine;

namespace SpherePainting
{
    public static class GraphicsBufferUtility
    {
        // ダミーのグラフィックスバッファを作成
        public static GraphicsBuffer CreateDummyGraphicsBuffer()
        {
            return new GraphicsBuffer(GraphicsBuffer.Target.Structured, 1, 4);
        }
    }
}