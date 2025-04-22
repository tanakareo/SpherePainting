using UnityEngine;

namespace SpherePainting
{
    [CreateAssetMenu(fileName = "CanvasMesh", menuName = "SpherePainting/CanvasMesh")]
    public class CanvasMesh : ScriptableObject
    {
        [field:SerializeField] public Mesh BaseMesh { get; private set; }
        [field:SerializeField] public Mesh LayerMesh { get; private set; }
    }
}