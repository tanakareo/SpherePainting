using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace SpherePainting
{
    // 球のデータ
    [Serializable]
    public struct SphereData
    {
        [field:SerializeField] public Vector3 Position { get; set; }
        [field:SerializeField] public float Radius  { get; set; }
        [field:SerializeField] public SphereOperation Operation  { get; set; } // 演算の種類
        [field:SerializeField] public uint OperationTargetCount  { get; set; } // 演算対象の球の数
        [field:SerializeField] public uint OperationTargetStartIndex  { get; set; } // 演算対象の球の開始インデックス

        public static int GetSize()
        {
            return Marshal.SizeOf<SphereData>();
        }
    };

    // 演算対象の球のデータ
    [Serializable]
    public struct OperationTargetSphereData
    {
        [field:SerializeField] public Vector3 Offset { get; set; }
        [field:SerializeField] public float Radius  { get; set; }
        [field:SerializeField] public uint MaterialIndex { get; set; } // 演算対象の球のマテリアル番号
        [field:SerializeField] public uint OperationAreaMaterialIndex { get; set; } // 演算箇所のマテリアル番号

        public static int GetSize()
        {
            return Marshal.SizeOf<OperationTargetSphereData>();
        }
    };
}