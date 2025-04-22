using R3;
using UnityEngine;

namespace SpherePainting
{
    public class RenderingCameraGizmo : Gizmo
    {
        [SerializeField] private Camera m_RenderingCamera;
        [SerializeField] private RenderingCameraSetting m_RenderingCameraSetting;
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
            m_RenderingCameraSetting.OnCameraSettingChanged += () =>
            {
                if(m_IsActive.CurrentValue == false) return;
                UpdateVertices();
            };
        }

        // メッシュを生成
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

        // 頂点の位置を計算
        private Vector3[] CalcVertices()
        {
            Vector3 origin = m_RenderingCamera.transform.position;
            Vector3 forward = m_RenderingCamera.transform.forward;
            Vector3 right = m_RenderingCamera.transform.right;
            Vector3 up = m_RenderingCamera.transform.up;
            float nearClipPlaneRightScale, nearClipPlaneUpScale, farClipPlaneRightScale, farClipPlaneUpScale;
            
            if(m_RenderingCamera.orthographic)
            {
                nearClipPlaneRightScale = m_RenderingCamera.orthographicSize;
                nearClipPlaneUpScale = m_RenderingCamera.orthographicSize;
                farClipPlaneRightScale = m_RenderingCamera.orthographicSize;
                farClipPlaneUpScale = m_RenderingCamera.orthographicSize;
            }
            else
            {
                float radFieldOfView = m_RenderingCamera.fieldOfView * Mathf.Deg2Rad;
                nearClipPlaneRightScale = Mathf.Tan(radFieldOfView * 0.5f) * m_RenderingCamera.nearClipPlane;
                nearClipPlaneUpScale = Mathf.Tan(radFieldOfView * 0.5f) * m_RenderingCamera.nearClipPlane;
                farClipPlaneRightScale = Mathf.Tan(radFieldOfView * 0.5f) * m_RenderingCamera.farClipPlane;
                farClipPlaneUpScale = Mathf.Tan(radFieldOfView * 0.5f) * m_RenderingCamera.farClipPlane;
            }

            Vector3[] vertices = new Vector3[]
            {
                origin + right * nearClipPlaneRightScale + up * nearClipPlaneUpScale + forward * m_RenderingCamera.nearClipPlane,
                origin - right * nearClipPlaneRightScale + up * nearClipPlaneUpScale + forward * m_RenderingCamera.nearClipPlane,
                origin - right * nearClipPlaneRightScale - up * nearClipPlaneUpScale + forward * m_RenderingCamera.nearClipPlane,
                origin + right * nearClipPlaneRightScale - up * nearClipPlaneUpScale + forward * m_RenderingCamera.nearClipPlane,
                origin + right * farClipPlaneRightScale + up * farClipPlaneUpScale + forward * m_RenderingCamera.farClipPlane,
                origin - right * farClipPlaneRightScale + up * farClipPlaneUpScale + forward * m_RenderingCamera.farClipPlane,
                origin - right * farClipPlaneRightScale - up * farClipPlaneUpScale + forward * m_RenderingCamera.farClipPlane,
                origin + right * farClipPlaneRightScale - up * farClipPlaneUpScale + forward * m_RenderingCamera.farClipPlane
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
