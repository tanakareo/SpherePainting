using System;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace SpherePainting
{
    // 球の生成領域
    public class SphereGenerationBounds
    {
        public Vector3 Size { get; private set; } = new (20.0f, 20.0f, 20.0f);
        public Vector3 Position { get; private set;} = new (0.0f, 0.0f, 0.0f);

        public event Action<SphereGenerationBounds> OnValueChanged;
        
        public SphereGenerationBounds() {}

        // 生成領域の大きさを設定
        public void SetSize(Vector3 size)
        {
            Size = size;
            OnValueChanged?.Invoke(this);
        }
        
        // 生成領域の位置を設定
        public void SetPosition(Vector3 position)
        {
            Position = position;
            OnValueChanged?.Invoke(this);
        }

        // 生成領域の中のランダムな位置を取得
        public Vector3 GetRandomPosition(ref Random random)
        {
            Vector3 extent = Size * 0.5f;
            Vector3 position = Position + (Vector3)random.NextFloat3(-extent, extent);
            return position;
        }
    }
}