using System.Collections.Generic;
using UnityEngine;

namespace SpherePainting
{
    // マテリアルデータを保持するクラス
    public class SphereMaterialDataListContainer : MonoBehaviour
    {
        [SerializeField] private List<SphereMaterialData> m_DataList = new ();
        
        public List<SphereMaterialData> DataList => m_DataList;

        public uint DataCount => (uint)DataList.Count;

        public void Clear()
        {
            DataList.Clear();
        }

        public void AddData(SphereMaterialData data)
        {
            m_DataList.Add(data);
        }

        public SphereMaterialData GetData(int index)
        {
            return m_DataList[index];
        }
    }
}
