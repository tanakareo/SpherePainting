using R3;
using UnityEngine;

namespace SpherePainting
{
    [RequireComponent(typeof(MeshFilter))]
    public class SphereGenerationBoundsGizmo : Gizmo
    {
        [SerializeField] private SphereDataCreator m_SphereGenerator;
        private Material m_Material;
        private Mesh m_Mesh;
        private MeshFilter m_MeshFilter;
        private readonly ReactiveProperty<bool> m_IsActive = new (true);
        private readonly ReactiveProperty<float> m_Opacity = new (1.0f);
        public override ReadOnlyReactiveProperty<float> Opacity => m_Opacity;
        public override ReadOnlyReactiveProperty<bool> IsActive => m_IsActive;

        private void Awake()
        {
            m_MeshFilter = GetComponent<MeshFilter>();
            m_Mesh = GenerateMesh();
            m_MeshFilter.mesh = m_Mesh;
            m_Material = GetComponent<MeshRenderer>().material;
            m_Opacity.Value = m_Material.color.a;

            m_SphereGenerator.GenerationBounds.OnValueChanged += bounds => 
            {
                if(m_IsActive.Value == false) return;
                UpdateVertices();
            };
        }

        private Mesh GenerateMesh()
        {
            Vector3[] vertices = CalcVertices();
            Mesh mesh = new ();
            mesh.SetVertices(vertices);
            int[] indices = new [] {0, 1, 1, 2, 2, 3, 3, 0, 0, 4, 1, 5, 2, 6, 3, 7, 4, 5, 5, 6, 6, 7, 7, 4};
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            return mesh;
        }

        private void UpdateVertices()
        {
            Vector3[] vertices = CalcVertices();
            m_Mesh.SetVertices(vertices);
        }

        private Vector3[] CalcVertices()
        {
            Vector3 position = m_SphereGenerator.GenerationBounds.Position;
            Vector3 size = m_SphereGenerator.GenerationBounds.Size;
            Vector3[] vertices = new Vector3[]
            {
                new (position.x + size.x * 0.5f, position.y + size.y * 0.5f, position.z - size.z * 0.5f),
                new (position.x - size.x * 0.5f, position.y + size.y * 0.5f, position.z - size.z * 0.5f),
                new (position.x - size.x * 0.5f, position.y - size.y * 0.5f, position.z - size.z * 0.5f),
                new (position.x + size.x * 0.5f, position.y - size.y * 0.5f, position.z - size.z * 0.5f),
                new (position.x + size.x * 0.5f, position.y + size.y * 0.5f, position.z + size.z * 0.5f),
                new (position.x - size.x * 0.5f, position.y + size.y * 0.5f, position.z + size.z * 0.5f),
                new (position.x - size.x * 0.5f, position.y - size.y * 0.5f, position.z + size.z * 0.5f),
                new (position.x + size.x * 0.5f, position.y - size.y * 0.5f, position.z + size.z * 0.5f)
            };
            return vertices;
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
            if(active)
            {
                m_MeshFilter.mesh = m_Mesh;
                UpdateVertices();
            }
            else m_MeshFilter.mesh = null;
        }
    }
}
