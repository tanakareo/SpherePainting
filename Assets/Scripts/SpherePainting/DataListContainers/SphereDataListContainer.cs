using System.Collections.Generic;
using UnityEngine;

namespace SpherePainting
{
    // 球のデータを保持するクラス
    public class SphereDataListContainer : MonoBehaviour
    {
        [SerializeField] private List<SphereData> m_SphereDataList = new ();
        [SerializeField] private List<OperationTargetSphereData> m_OperationTargetSphereDataList = new ();
        
        public List<SphereData> SphereDataList => m_SphereDataList;
        public List<OperationTargetSphereData> OperationTargetSphereDataList => m_OperationTargetSphereDataList;

        public int SphereDataCount => m_SphereDataList.Count;
        public int OperationTargetSphereDataCount => m_OperationTargetSphereDataList.Count;

        public void ClearAll()
        {
            m_SphereDataList.Clear();
            m_OperationTargetSphereDataList.Clear();
        }

        public void AddSphereData(SphereData data)
        {
            m_SphereDataList.Add(data);
        }

        public SphereData GetSphereData(int index)
        {
            return m_SphereDataList[index];
        }

        public void AddOperationTargetSphereData(OperationTargetSphereData data)
        {
            m_OperationTargetSphereDataList.Add(data);
        }
    }
}
