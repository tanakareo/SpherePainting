using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using WeightedValues;
using Range;

namespace SpherePainting
{
    public static class SphereDataCreatorDataHandler
    {
        [Serializable]
        public class SphereDataCreatorData
        {
            [JsonProperty] public uint Seed { get; private set; }
            [JsonProperty] public uint SphereCount { get; private set; }
            [JsonProperty] public Vector3 GenerationBoundsSize { get; private set; }
            [JsonProperty] public Vector3 GenerationBoundsPosition { get; private set; }
            [JsonProperty] public Dictionary<SphereOperation, Vector2> SphereRadiusRange { get; private set; } = new ();
            [JsonProperty] public Dictionary<SphereOperation, Vector2> OperationTargetRadiusRange { get; private set; } = new ();
            [JsonProperty] public Dictionary<SphereOperation, Vector2Int> OperationTargetCountRange { get; private set; } = new ();
            [JsonProperty] public WeightedItem<SphereOperation>[] WeightedSphereOperations { get; private set; }

            public SphereDataCreatorData(){}

            public SphereDataCreatorData(SphereDataCreator sphereDataCreator)
            {
                Seed = sphereDataCreator.Seed.CurrentValue;
                SphereCount = sphereDataCreator.SphereCount.CurrentValue;
                GenerationBoundsSize = sphereDataCreator.GenerationBounds.Size;
                GenerationBoundsPosition = sphereDataCreator.GenerationBounds.Position;
                foreach(var (operation, factory) in sphereDataCreator.SphereFactories)
                {
                    SphereRadiusRange.Add(operation, factory.RadiusRange.ToVector2());

                    if(factory is not OperatedSphereFactory operatedSphereFactory) continue;

                    OperationTargetRadiusRange.Add(operation, operatedSphereFactory.OperationTargetRadiusRange.ToVector2());
                    OperationTargetCountRange.Add(operation, operatedSphereFactory.OperationTargetCountRange.ToVector2Int());
                }
                WeightedSphereOperations = sphereDataCreator.WeightedSphereOperations.Weights.ToArray();
            }
        }

        public static SphereDataCreatorData ToData(this SphereDataCreator sphereDataCreator)
        {
            SphereDataCreatorData data = new (sphereDataCreator);
            return data;
        }

        public static void SetData(this SphereDataCreator sphereDataCreator, SphereDataCreatorData data)
        {
            sphereDataCreator.SetSeed(data.Seed);
            sphereDataCreator.SetSphereCount(data.SphereCount);
            sphereDataCreator.GenerationBounds.SetSize(data.GenerationBoundsSize);
            sphereDataCreator.GenerationBounds.SetPosition(data.GenerationBoundsPosition);
            foreach(var (operation, factory) in sphereDataCreator.SphereFactories)
            {
                Vector2 radiusRange = data.SphereRadiusRange[operation];
                factory.RadiusRange.Set(radiusRange);

                if(factory is not OperatedSphereFactory operatedSphereFactory) continue;

                Vector2 operationTargetRadiusRange = data.OperationTargetRadiusRange[operation];
                operatedSphereFactory.OperationTargetRadiusRange.Set(operationTargetRadiusRange);
                Vector2Int operationTargetCountRange = data.OperationTargetCountRange[operation];
                operatedSphereFactory.OperationTargetCountRange.Set(operationTargetCountRange);
            }
            sphereDataCreator.WeightedSphereOperations.SetWeights(data.WeightedSphereOperations);
        }
    }
}