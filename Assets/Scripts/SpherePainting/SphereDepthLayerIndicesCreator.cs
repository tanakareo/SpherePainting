using System;
using System.Linq;
using UnityEngine;

namespace SpherePainting
{
    // カメラからの距離に基づいて球のレイヤーインデックスを作成するクラス
    public class SphereDepthLayerIndicesCreator : MonoBehaviour
    {
        [SerializeField] private SphereDataListContainer m_SphereDataListContainer;

        // カメラのトランスフォームから球のレイヤーインデックスを作成する
        public int[] Create(Transform cameraTransform)
        {
            int sphereCount = m_SphereDataListContainer.SphereDataCount;
            int[] layerIndices = new int[sphereCount];
            int[] indices = new int[sphereCount];
            float[] sphereDistances = new float[sphereCount];
            for(int i = 0; i < sphereCount; ++i)
            {
                indices[i] = i;
                SphereData sphereData = m_SphereDataListContainer.GetSphereData(i);
                sphereDistances[i] = Vector3.Dot(sphereData.Position - cameraTransform.position, cameraTransform.forward);
            }
            indices = indices.OrderBy(i => sphereDistances[i]).ToArray();

            for(int i = 0; i < sphereCount; ++i) layerIndices[indices[i]] = i;
            
            return layerIndices;
        }
    }
}