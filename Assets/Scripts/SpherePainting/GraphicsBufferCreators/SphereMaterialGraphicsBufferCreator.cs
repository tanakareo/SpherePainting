using System;
using UnityEngine;

namespace SpherePainting
{
    public class SphereMaterialGraphicsBufferCreator : MonoBehaviour
    {
        private GraphicsBuffer m_Buffer;
        public GraphicsBuffer Buffer
        {
            get
            {
                if(m_Buffer == null)
                {
                    m_Buffer = CreateBuffer();
                    OnBufferCreated?.Invoke();
                }

                return m_Buffer;
            }
        }

        public event Action OnBufferCreated;

        [SerializeField] private SphereMaterialDataListContainer m_SphereMaterialDataListContainer;

        private void OnDestroy()
        {
            DisposeBuffers();
        }

        public void UpdateBuffer()
        {
            m_Buffer?.Dispose();
            m_Buffer = CreateBuffer();
            OnBufferCreated?.Invoke();
        }

        private GraphicsBuffer CreateBuffer()
        {
            int count = (int)m_SphereMaterialDataListContainer.DataCount;

            GraphicsBuffer graphicsBuffer;
            if(count <= 0)
            {
                graphicsBuffer = GraphicsBufferUtility.CreateDummyGraphicsBuffer();
            }
            else
            {
                graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, SphereMaterialData.GetSize());
                graphicsBuffer.SetData(m_SphereMaterialDataListContainer.DataList);
            }

            return graphicsBuffer;
        }

        private void DisposeBuffers()
        {
            m_Buffer?.Dispose();
        }
    }
}
