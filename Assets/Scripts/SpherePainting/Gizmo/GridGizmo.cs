using System.Collections.Generic;
using R3;
using UnityEngine;

namespace SpherePainting
{
    public class GridGizmo : Gizmo
    {
        [SerializeField] private Vector2Int m_CellCount = new Vector2Int(100, 100);
        [SerializeField] private float m_CellSize = 10.0f;
        [SerializeField] private float m_Height = 0.0f;
        private Material m_Material;
        private Vector2 Size => (Vector2)m_CellCount * m_CellSize;
        private Mesh m_Mesh;
        private MeshFilter m_MeshFilter;
        private List<Vector3> m_Vertices = new ();
        private List<int> m_Indices = new ();
        private readonly ReactiveProperty<float> m_Opacity = new (1.0f);
        private readonly ReactiveProperty<bool> m_IsActive = new (true);
        public override ReadOnlyReactiveProperty<float> Opacity => m_Opacity;
        public override ReadOnlyReactiveProperty<bool> IsActive => m_IsActive;

        private void Awake()
        {
            m_MeshFilter = GetComponent<MeshFilter>();
            m_Mesh = GenerateMesh();
            m_MeshFilter.mesh = m_Mesh;
            m_Material = GetComponent<MeshRenderer>().material;
            m_Opacity.Value = m_Material.color.a;
        }

        // メッシュを生成
        private Mesh GenerateMesh()
        {
            Mesh mesh = new ();
            m_Vertices.Clear();
            m_Indices.Clear();
            Vector3 startPos = new Vector3(m_CellCount.x * m_CellSize, 0, m_CellCount.y * m_CellSize) * -0.5f + Vector3.up * m_Height;
            int index = 0;
            for(int i = 0; i <= m_CellCount.x; ++i)
            {
                m_Vertices.Add(startPos + Vector3.right * i * m_CellSize);
                m_Vertices.Add(startPos + Vector3.right * i * m_CellSize + Vector3.forward * Size.x);

                m_Indices.Add(index);
                index ++;
                m_Indices.Add(index);
                index ++;                
            }
            for(int i = 0; i <= m_CellCount.y; ++i)
            {
                m_Vertices.Add(startPos + Vector3.forward * i * m_CellSize);
                m_Vertices.Add(startPos + Vector3.forward * i * m_CellSize + Vector3.right * Size.y);
                
                m_Indices.Add(index);
                index ++;
                m_Indices.Add(index);
                index ++;
            }

            mesh.SetVertices(m_Vertices);
            mesh.SetIndices(m_Indices, MeshTopology.Lines, 0);
            return mesh;
        }

        // 不透明度を設定
        public override void SetOpacity(float opacity)
        {
            m_Opacity.Value = opacity;
            Color color = m_Material.color;
            color.a = opacity;
            m_Material.color = color;
        }

        // 表示するかどうかを設定
        public override void SetActive(bool active)
        {
            m_IsActive.Value = active;
            if(active) m_MeshFilter.mesh = m_Mesh;
            else m_MeshFilter.mesh = null;
        }
    }
}
