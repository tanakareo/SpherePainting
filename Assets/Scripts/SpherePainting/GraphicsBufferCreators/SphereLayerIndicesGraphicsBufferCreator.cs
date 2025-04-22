using System;
using UnityEngine;

namespace SpherePainting
{
    public class SphereLayerIndicesGraphicsBufferCreator : MonoBehaviour
    {
        [SerializeField] private SphereDataListContainer m_SphereDataListContainer;
        [SerializeField] private SphereDepthLayerIndicesCreator m_SphereDepthLayerIndicesCreator;

        private GraphicsBuffer m_DepthLayerBuffer;
        public GraphicsBuffer DepthLayerBuffer
        {
            get
            {
                return m_DepthLayerBuffer;
            }
        }

        private GraphicsBuffer m_SingleLayerBuffer;
        public GraphicsBuffer SingleLayerBuffer
        {
            get
            {
                if(m_SingleLayerBuffer == null)
                {
                    m_SingleLayerBuffer = CreateSingleLayerBuffer();
                }
                return m_SingleLayerBuffer;
            }
        }

        private void OnDestroy()
        {
            m_SingleLayerBuffer?.Dispose();
            m_DepthLayerBuffer?.Dispose();
        }

        private GraphicsBuffer CreateSingleLayerBuffer()
        {
            int[] layerIndices = new int[m_SphereDataListContainer.SphereDataCount];
            Array.Fill(layerIndices, 0);
            GraphicsBuffer buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, m_SphereDataListContainer.SphereDataCount, sizeof(int));
            buffer.SetData(layerIndices);
            return buffer;
        }

        public void UpdateSingleLayerBuffer()
        {
            m_SingleLayerBuffer?.Dispose();
            m_SingleLayerBuffer = CreateSingleLayerBuffer();
        }

        public void UpdateDepthLayerBuffer(Transform cameraTransform)
        {
            m_DepthLayerBuffer?.Dispose();
            m_DepthLayerBuffer = CreateDepthLayerBuffer(cameraTransform);
        }

        public GraphicsBuffer CreateDepthLayerBuffer(Transform cameraTransform)
        {
            GraphicsBuffer buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, m_SphereDataListContainer.SphereDataCount, sizeof(int));
            buffer.SetData(m_SphereDepthLayerIndicesCreator.Create(cameraTransform));
            return buffer;
        }
    }
}