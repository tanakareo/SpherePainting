using UnityEngine;
using Random = Unity.Mathematics.Random;
using Range;

namespace SpherePainting
{
    // 球のファクトリーのインターフェース
    public interface ISphereFactory
    {
        public ReactiveRange<float> RadiusRange { get;}
        public SphereData GetRandomSphereData(Vector3 position, ref Random random);
    }

    // 球の演算の種類
    public enum SphereOperation { NORMAL = 0, CAP = 1, DIFFERENCE = 2}

    // ランダムな球のデータを生成するクラス
    public class SphereFactory : ISphereFactory
    {
        public ReactiveRange<float> RadiusRange { get; private set; }

        public SphereFactory(ReactiveRange<float> radiusRange)
        {
            RadiusRange = radiusRange;
        }

        public SphereData GetRandomSphereData(Vector3 position, ref Random random)
        {
            SphereData data = new ()
            {
                Position = position,
                Radius = RadiusRange.GetRandomValue(ref random),
                Operation = SphereOperation.NORMAL,
                OperationTargetCount = 0,
                OperationTargetStartIndex = 0,
            };
            return data;
        }
    }

    // ランダムな演算対象の球を生成するクラス
    public class OperatedSphereFactory : ISphereFactory
    {
        public ReactiveRange<float> RadiusRange { get; private set; }
        public ReactiveRange<float> OperationTargetRadiusRange { get; private set; }
        public ReactiveRange<uint> OperationTargetCountRange { get; private set; }

        private readonly SphereOperation m_Operation;

        public OperatedSphereFactory(ReactiveRange<float> radiusRange, 
                                    ReactiveRange<float> operationTargetRadiusRange, 
                                    ReactiveRange<uint> operationTargetCountRange, 
                                    SphereOperation operation)
        {
            RadiusRange = radiusRange;
            OperationTargetRadiusRange = operationTargetRadiusRange;
            OperationTargetCountRange = operationTargetCountRange;
            m_Operation = operation;
        }

        public SphereData GetRandomSphereData(Vector3 position, ref Random random)
        {
            uint operationTargetCount = OperationTargetCountRange.GetRandomValue(ref random);
            SphereData data = new ()
            {
                Position = position,
                Radius = RadiusRange.GetRandomValue(ref random),
                Operation = m_Operation,
                OperationTargetCount = operationTargetCount,
                OperationTargetStartIndex = 0,
            };
            return data;
        }

        public OperationTargetSphereData GetRandomOperationTargetSphereData(SphereData sphereData, ref Random random)
        {
            float radius = sphereData.Radius * OperationTargetRadiusRange.GetRandomValue(ref random);
            float offsetDist = random.NextFloat(0.0f, sphereData.Radius + radius);
            Vector3 offset = offsetDist * Vector3Utility.GetRandomPointOnUnitSphere(ref random);
            OperationTargetSphereData data = new ()
            {
                Offset = offset,
                Radius = radius,
                MaterialIndex = 0,
                OperationAreaMaterialIndex = 0
            };
            return data;
        }
    }
}