using UnityEngine;

namespace SpherePainting
{
    public class SpiralTileAreaUpdater : ITileAreaUpdater
    {
        private readonly Vector2Int m_Resolution;
        public Vector2Int Resolution => m_Resolution;
        private readonly Vector2Int m_TileSize;
        public Vector2Int TileSize => m_TileSize;
        public TileArea CurrentTileArea => m_TileAreas[m_ProcessedTiles];
        private readonly int m_TotalTileCount;
        private readonly Vector2Int m_TotalTiles;
        private int m_ProcessedTiles;
        public float Progress => (float)m_ProcessedTiles / m_TotalTileCount;
        private readonly TileArea[] m_TileAreas;

        public SpiralTileAreaUpdater(Vector2Int resolution, Vector2Int tileSize)
        {
            m_ProcessedTiles = 0;
            m_Resolution = resolution;
            m_TileSize = tileSize;
            m_TotalTiles = new Vector2Int(Mathf.CeilToInt((float)resolution.x / tileSize.x),
                                          Mathf.CeilToInt((float)resolution.y / tileSize.y));
            m_TotalTileCount = m_TotalTiles.x * m_TotalTiles.y;
            m_TileAreas = CreateTileAreas();
        }

        public void Reset()
        {
            m_ProcessedTiles = 0;
        }

        public void Update()
        {
            if(IsCompleted()) return;
            m_ProcessedTiles++;
        }

        public bool IsCompleted()
        {
            return m_ProcessedTiles >= m_TotalTileCount;
        }

        private TileArea[] CreateTileAreas()
        {
            Vector2Int bottomLeft = Vector2Int.FloorToInt((Vector2)(m_TotalTiles - Vector2Int.one) * 0.5f);
            Vector2Int topRight = Vector2Int.FloorToInt((Vector2)m_TotalTiles * 0.5f);
            Vector2Int[] tilePositions = new Vector2Int[m_TotalTileCount];
            bool[,] isVisited = new bool[m_TotalTiles.x, m_TotalTiles.y];
            int tileCount = 0;
            while(tileCount < m_TotalTileCount)
            {
                // １行のみの時
                if(bottomLeft.y == topRight.y)
                {
                    for(int x = bottomLeft.x; x <= topRight.x; ++x)
                    {
                        if(isVisited[x, bottomLeft.y]) continue;
                        isVisited[x, bottomLeft.y] = true;
                        tilePositions[tileCount] = new Vector2Int(x, bottomLeft.y);
                        tileCount++;
                    }
                }
                // １列のみの時
                else if(bottomLeft.x == topRight.x)
                {
                    for(int y = bottomLeft.y; y <= topRight.y; ++y)
                    {
                        if(isVisited[bottomLeft.x, y]) continue;
                        isVisited[bottomLeft.x, y] = true;
                        tilePositions[tileCount] = new Vector2Int(bottomLeft.x, y);
                        tileCount++;
                    }
                }
                else
                {
                    for(int y = bottomLeft.y; y <= topRight.y; ++y)
                    {
                        if(isVisited[bottomLeft.x, y]) continue;
                        isVisited[bottomLeft.x, y] = true;
                        tilePositions[tileCount] = new Vector2Int(bottomLeft.x, y);
                        tileCount++;
                    }

                    for(int x = bottomLeft.x + 1; x <= topRight.x; ++x)
                    {
                        if(isVisited[x, topRight.y]) continue;
                        isVisited[x, topRight.y] = true;
                        tilePositions[tileCount] = new Vector2Int(x, topRight.y);
                        tileCount++;
                    }

                    for(int y = topRight.y - 1; y >= bottomLeft.y; --y)
                    {
                        if(isVisited[topRight.x, y]) continue;
                        isVisited[topRight.x, y] = true;
                        tilePositions[tileCount] = new Vector2Int(topRight.x, y);
                        tileCount++;
                    }

                    for(int x = topRight.x - 1; x >= bottomLeft.x + 1; --x)
                    {
                        if(isVisited[x, bottomLeft.y]) continue;
                        isVisited[x, bottomLeft.y] = true;
                        tilePositions[tileCount] = new Vector2Int(x, bottomLeft.y);
                        tileCount++;
                    }
                }

                bottomLeft = Vector2Int.Max(Vector2Int.zero, bottomLeft - Vector2Int.one);
                topRight = Vector2Int.Min(m_TotalTiles - Vector2Int.one, topRight + Vector2Int.one);
            }

            TileArea[] tileAreas = new TileArea[m_TotalTileCount];
            Vector2Int overflow = Vector2Int.CeilToInt((Vector2)(m_TotalTiles * m_TileSize - m_Resolution) * 0.5f);
            for(int i = 0; i < tilePositions.Length; ++i)
            {
                Vector2Int tileOffset = Vector2Int.Max(Vector2Int.zero, tilePositions[i] * m_TileSize - overflow);
                Vector2Int tileSize = Vector2Int.Min(m_Resolution, tileOffset + m_TileSize) - tileOffset;
                tileAreas[i] = new TileArea(tileOffset, tileSize);
            }
            return tileAreas;
        }
    }
}