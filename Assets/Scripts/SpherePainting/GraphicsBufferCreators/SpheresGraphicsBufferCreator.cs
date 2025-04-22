using System;
using UnityEngine;

namespace SpherePainting
{
    public class SpheresGraphicsBufferCreator : MonoBehaviour
    {
        private GraphicsBuffer m_SpheresBuffer;
        public GraphicsBuffer SpheresBuffer
        {
            get
            {
                if(m_SpheresBuffer == null)
                {
                    m_SpheresBuffer = CreateSpheresBuffer();
                    OnSpheresBufferCreated?.Invoke();
                }

                return m_SpheresBuffer;
            }
        }
        private GraphicsBuffer m_OperationTargetSpheresBuffer;
        public GraphicsBuffer OperationTargetSpheresBuffer
        {
            get
            {
                if(m_OperationTargetSpheresBuffer == null)
                {
                    m_OperationTargetSpheresBuffer = CreateOperationTargetSpheresBuffer();
                    OnOperationTargetSpheresBufferCreated?.Invoke();
                }

                return m_OperationTargetSpheresBuffer;
            }
        }

        public event Action OnSpheresBufferCreated;
        public event Action OnOperationTargetSpheresBufferCreated;

        [SerializeField] private SphereDataListContainer m_SphereDataListContainer;

        private bool m_IsUpdateSpheresBufferRequired = false;
        private bool m_IsUpdateOperationTargetSpheresBufferRequired = false;

        public void RequireUpdateSpheresBuffer()
        {
            m_IsUpdateSpheresBufferRequired = true;
        }

        public void RequireUpdateOperationTargetSpheresBuffer()
        {
            m_IsUpdateOperationTargetSpheresBufferRequired = true;
        }

        private void OnDestroy()
        {
            DisposeBuffers();
        }

        private void Update()
        {
            if(m_IsUpdateSpheresBufferRequired)
            {
                UpdateSpheresBuffer();
            }

            if(m_IsUpdateOperationTargetSpheresBufferRequired)
            {
                UpdateOperationTargetSpheresBuffer();
            }
        }

        private void UpdateSpheresBuffer()
        {
            m_SpheresBuffer?.Dispose();
            m_SpheresBuffer = CreateSpheresBuffer();
            OnSpheresBufferCreated?.Invoke();
            m_IsUpdateSpheresBufferRequired = false;
        }

        private void UpdateOperationTargetSpheresBuffer()
        {
            m_OperationTargetSpheresBuffer?.Dispose();
            m_OperationTargetSpheresBuffer = CreateOperationTargetSpheresBuffer();
            OnOperationTargetSpheresBufferCreated?.Invoke();
            m_IsUpdateOperationTargetSpheresBufferRequired = false;
        }

        private GraphicsBuffer CreateSpheresBuffer()
        {
            int count = m_SphereDataListContainer.SphereDataCount;

            GraphicsBuffer graphicsBuffer;
            if(count <= 0)
            {
                graphicsBuffer = GraphicsBufferUtility.CreateDummyGraphicsBuffer();
            }
            else
            {
                graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, SphereData.GetSize());
                graphicsBuffer.SetData(m_SphereDataListContainer.SphereDataList);
            }

            return graphicsBuffer;
        }

        private GraphicsBuffer CreateOperationTargetSpheresBuffer()
        {
            int count = m_SphereDataListContainer.OperationTargetSphereDataCount;

            GraphicsBuffer graphicsBuffer;
            if(count <= 0)
            {
                graphicsBuffer = GraphicsBufferUtility.CreateDummyGraphicsBuffer();
            }
            else
            {
                graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, OperationTargetSphereData.GetSize());
                graphicsBuffer.SetData(m_SphereDataListContainer.OperationTargetSphereDataList);
            }

            return graphicsBuffer;
        }

        private void DisposeBuffers()
        {
            m_SpheresBuffer?.Dispose();
            m_OperationTargetSpheresBuffer?.Dispose();
        }
    }
}