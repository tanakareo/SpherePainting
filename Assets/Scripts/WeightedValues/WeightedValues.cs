using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace WeightedValues
{
    public class WeightedItem<T>
    {
        // 重みの最小値
        private static readonly float MIN_WEIGHT = 0.0001f;
        public T Value { get; private set; }

        private float m_Weight;
        public float Weight
        { 
            get => m_Weight;
            internal set
            {
                if (value < 0)
                    throw new ArgumentException("Weight must be non-negative.");

                m_Weight = Mathf.Max(MIN_WEIGHT, value);
            }
        }
        
        public WeightedItem(T value, float weight)
        {
            if (weight < 0)
                throw new ArgumentException("Weight must be non-negative.");
            
            Value = value;
            Weight = weight;
        }
    }

    public interface IWeightedValues<T>
    {
        public IReadOnlyList<WeightedItem<T>> Weights { get; }
        public float GetWeight(T value);
        public void SetWeight(T value, float weight);
        public void SetNormalizedWeight(T value, float newWeight);
        public T GetRandomValue(ref Random random);
    }

    public class WeightedValues<T> : IWeightedValues<T>
    {
        private readonly List<WeightedItem<T>> m_Weights = new ();
        public IReadOnlyList<WeightedItem<T>> Weights => m_Weights;
        private bool m_NeedsNormalization = true;
        private float[] m_CumulativeWeights;

        public WeightedValues(params WeightedItem<T>[] items)
        {
            AddValues(items);
        }

        // 値を追加
        public void Add(WeightedItem<T> item)
        {
            if (m_Weights.Any(x => EqualityComparer<T>.Default.Equals(x.Value, item.Value)))
                throw new ArgumentException("Value already exists.");
            
            m_Weights.Add(item);
            m_NeedsNormalization = true;
        }

        // 複数の値を追加
        public void AddValues(params WeightedItem<T>[] items)
        {
            foreach (var item in items)
            {
                if (m_Weights.Any(x => EqualityComparer<T>.Default.Equals(x.Value, item.Value)))
                    throw new ArgumentException("Value already exists.");

                m_Weights.Add(item);
            }
            m_NeedsNormalization = true;
        }

        // 値を削除
        public void Remove(T value)
        {
            m_Weights.RemoveAll(x => EqualityComparer<T>.Default.Equals(x.Value, value));
            m_NeedsNormalization = true;
        }

        // 重みを更新
        public void SetWeight(T value, float newWeight)
        {
            var item = m_Weights.FirstOrDefault(x => EqualityComparer<T>.Default.Equals(x.Value, value));
            if (item == null) throw new KeyNotFoundException("Value not found.");

            item.Weight = newWeight;
            m_NeedsNormalization = true;
        }

        // [0, 1]で重みを設定
        public void SetNormalizedWeight(T value, float newWeight)
        {
            if (newWeight < 0.0f || newWeight > 1.0f) throw new ArgumentException("Weight is not normalized.");
            if(m_NeedsNormalization) NormalizeWeights();

            float currentWeight = GetWeight(value);
            float totalWeight = m_Weights.Sum(item => item.Weight);
            for (int i = 0; i < m_Weights.Count; ++i)
            {
                if(EqualityComparer<T>.Default.Equals(m_Weights[i].Value, value))
                {
                    m_Weights[i].Weight = newWeight;
                    continue;
                }
                m_Weights[i].Weight *= (totalWeight - newWeight) / (totalWeight - currentWeight);
                m_Weights[i].Weight = Mathf.Min(1.0f, m_Weights[i].Weight);
            }
            RecalculateCumulativeWeights();
        }

        // 合計が1.0になるように重みを正規化
        private void NormalizeWeights()
        {
            float totalWeight = m_Weights.Sum(item => item.Weight);

            if (totalWeight <= 0) return;

            for (int i = 0; i < m_Weights.Count; ++i)
            {
                m_Weights[i].Weight /= totalWeight;
            }
            RecalculateCumulativeWeights();
            m_NeedsNormalization = false;
        }

        // 指定した値の重みを取得
        public float GetWeight(T value)
        {
            var item = m_Weights.FirstOrDefault(x => EqualityComparer<T>.Default.Equals(x.Value, value));
            if (item == null) throw new KeyNotFoundException("Value not found.");
            if(m_NeedsNormalization) NormalizeWeights();
            
            return item.Weight;
        }
        
        // 重みの合計を再計算
        private void RecalculateCumulativeWeights()
        {
            m_CumulativeWeights = new float[m_Weights.Count];

            if(m_Weights.Count <= 0) return;

            m_CumulativeWeights[0] = m_Weights[0].Weight;
            for (int i = 1; i < m_Weights.Count; ++i)
            {
                m_CumulativeWeights[i] = m_CumulativeWeights[i-1] + m_Weights[i].Weight;
            }
        }

        // ランダムな値を取得
        public T GetRandomValue(ref Random random)
        {
            if (m_NeedsNormalization) NormalizeWeights();

            float rand = random.NextFloat();
            int index = Array.BinarySearch(m_CumulativeWeights, rand);
            if (index < 0) index = ~index; // BinarySearch returns negative index if not found.

            return m_Weights[index].Value;
        }
    }
}
