using System.Collections;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace SpherePainting
{
    // レンダリング結果を保持するクラス
    public class RenderResult : MonoBehaviour, IEnumerable<RenderTexture>
    {
        private readonly List<RenderTexture> m_RenderTextures = new List<RenderTexture>();
        public IReadOnlyList<RenderTexture> RenderTextures => m_RenderTextures;

        private Vector2Int m_Resolution;
        public Vector2Int Resolution => m_Resolution;

        private readonly ReactiveProperty<bool> m_IsEmpty = new (true);
        public ReadOnlyReactiveProperty<bool> IsEmpty => m_IsEmpty;
        public int LayerCount => m_RenderTextures.Count;

        public void Init(int layerCount, Vector2Int resolution)
        {
            m_Resolution = resolution;
            Release();
            m_RenderTextures.Clear();
            
            for(int i = 0; i < layerCount; ++i)
            {
                RenderTexture newRenderTexture = new RenderTexture(resolution.x, resolution.y, 0,
                                                                    RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB)
                {
                    enableRandomWrite = true
                };
                newRenderTexture.Create();
                m_RenderTextures.Add(newRenderTexture);
            }

            m_IsEmpty.Value = m_RenderTextures.Count == 0;
        }

        public void CopyTexture(RenderTexture source, int layer)
        {
            if (layer < 0 || layer >= m_RenderTextures.Count)
            {
                Debug.LogWarning($"Layer index {layer} is out of range.");
                return;
            }
            Graphics.CopyTexture(source, m_RenderTextures[layer]);
        }

        private void Release()
        {
            foreach(var texture in m_RenderTextures)
            {
                texture.Release();
            }
        }

        private void OnDestroy()
        {
            Release();
        }

        public IEnumerator<RenderTexture> GetEnumerator()
        {
            return m_RenderTextures.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}