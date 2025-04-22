using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace SpherePainting
{
    public static class Vector3Utility
    {
        // ランダムな単位ベクトルを返す
        public static Vector3 GetRandomPointOnUnitSphere(ref Random random)
        {
            float cosTheta = -2.0f * random.NextFloat() + 1.0f;
            float sinTheta = Mathf.Sqrt(1.0f - cosTheta * cosTheta);
            float phi = 2.0f * Mathf.PI * random.NextFloat();
            Vector3 randomPosition = new Vector3(sinTheta * Mathf.Cos(phi), sinTheta * Mathf.Sin(phi), cosTheta);
            return randomPosition;
        }
    }
}