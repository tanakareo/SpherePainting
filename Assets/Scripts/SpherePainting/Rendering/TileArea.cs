using UnityEngine;

namespace SpherePainting
{
    public struct TileArea
    {
        public Vector2Int Offset { get; }
        public Vector2Int Size { get; }

        public TileArea(Vector2Int offset, Vector2Int size)
        {
            Offset = offset;
            Size = size;
        }
    }
}