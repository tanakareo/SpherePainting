using System;
using R3;
using Random = Unity.Mathematics.Random;
using UnityEngine;
using Unity.Mathematics;

namespace Range
{
    // 監視可能なRange
    public class ReactiveRange<T> where T : IComparable<T>
    {
        private readonly ReactiveProperty<Range<T>> m_Range;
        public T Min => m_Range.Value.Min;
        public T Max => m_Range.Value.Max;

        public ReactiveRange(T minLimit, T maxLimit, T min, T max)
        {
            m_Range = new ( new Range<T>(minLimit, maxLimit, min, max));
        }

        public IDisposable Subscribe(Action<Range<T>> onNext)
        {
            return m_Range.Subscribe(onNext);
        }

        public void Set(T min, T max)
        {
            m_Range.Value.Set(min, max);
            m_Range.OnNext(m_Range.Value);
        }
    }

    public static class FloatReactiveRangeExtensions
    {
        public static float GetRandomValue(this ReactiveRange<float> range, ref Random random)
        {
            return random.NextFloat(range.Min, range.Max);
        }

        public static Vector2 ToVector2(this ReactiveRange<float> range)
        {
            return new Vector2(range.Min, range.Max);
        }

        public static void Set(this ReactiveRange<float> range, Vector2 minMaxRange)
        {
            range.Set(minMaxRange.x, minMaxRange.y);
        }
    }

    public static class IntReactiveRangeExtensions
    {
        public static int GetRandomValue(this ReactiveRange<int> range, ref Random random)
        {
            return random.NextInt(range.Min, range.Max + 1);
        }

        public static Vector2 ToVector2(this ReactiveRange<int> range)
        {
            return new Vector2(range.Min, range.Max);
        }

        public static Vector2Int ToVector2Int(this ReactiveRange<int> range)
        {
            return new Vector2Int(range.Min, range.Max);
        }

        public static void Set(this ReactiveRange<int> range, Vector2Int minMaxRange)
        {
            range.Set(minMaxRange.x, minMaxRange.y);
        }
    }

    public static class UIntReactiveRangeExtensions
    {
        public static uint GetRandomValue(this ReactiveRange<uint> range, ref Random random)
        {
            return random.NextUInt(range.Min, range.Max + 1);
        }

        public static Vector2 ToVector2(this ReactiveRange<uint> range)
        {
            return new Vector2(range.Min, range.Max);
        }

        public static Vector2Int ToVector2Int(this ReactiveRange<uint> range)
        {
            return new Vector2Int((int)range.Min, (int)range.Max);
        }

        public static uint2 ToUInt2(this ReactiveRange<uint> range)
        {
            return new uint2(range.Min, range.Max);
        }

        public static void Set(this ReactiveRange<uint> range, Vector2Int minMaxRange)
        {
            range.Set((uint)minMaxRange.x, (uint)minMaxRange.y);
        }

        public static void Set(this ReactiveRange<uint> range, uint2 minMaxRange)
        {
            range.Set(minMaxRange.x, minMaxRange.y);
        }
    }
}