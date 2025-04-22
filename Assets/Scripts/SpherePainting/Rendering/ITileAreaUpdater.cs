using UnityEngine;

namespace SpherePainting
{
    public interface ITileAreaUpdater
    {
        public float Progress { get; }
        public TileArea CurrentTileArea { get; }
        public Vector2Int Resolution { get; }
        public Vector2Int TileSize { get; }
        public void Reset();
        public void Update();
        public bool IsCompleted();
    }
}