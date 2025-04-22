using System;
using System.Collections.Generic;
using R3;
using Random = Unity.Mathematics.Random;

namespace WeightedValues
{
    // 監視可能なWeightedValues
    public class ReactiveWeightedValues<T> : IWeightedValues<T>
    {
        private readonly ReactiveProperty<WeightedValues<T>> m_WeightedValues;
        public IReadOnlyList<WeightedItem<T>> Weights => m_WeightedValues.Value.Weights;

        public IDisposable Subscribe(Action<WeightedValues<T>> onNext)
        {
            return m_WeightedValues.Subscribe(onNext);
        }

        public ReactiveWeightedValues(params WeightedItem<T>[] items)
        {
            m_WeightedValues = new (new (items));
        }
        
        public float GetWeight(T value)
        {
            return m_WeightedValues.Value.GetWeight(value);
        }

        public void SetWeight(T value, float newWeight)
        {
            m_WeightedValues.Value.SetWeight(value, newWeight);
            m_WeightedValues.OnNext(m_WeightedValues.Value);
        }

        public void SetNormalizedWeight(T value, float newWeight)
        {
            m_WeightedValues.Value.SetNormalizedWeight(value, newWeight);
            m_WeightedValues.OnNext(m_WeightedValues.Value);
        }

        public void SetWeights(params WeightedItem<T>[] items)
        {
            foreach(var item in items)
            {
                m_WeightedValues.Value.SetWeight(item.Value, item.Weight);
            }
            m_WeightedValues.OnNext(m_WeightedValues.Value);
        }

        public T GetRandomValue(ref Random random)
        {
            return m_WeightedValues.Value.GetRandomValue(ref random);
        }
    }
}
