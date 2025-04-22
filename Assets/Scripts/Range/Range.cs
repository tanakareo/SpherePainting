using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = Unity.Mathematics.Random;

namespace Range
{
    public class Range<T> where T : IComparable<T>
    {
        private T m_Min, m_Max;

        public T Min
        {
            get => m_Min;

            set
            {
                if(EqualityComparer<T>.Default.Equals(m_Min, value)) return;
                
                if(value.CompareTo(m_MinLimit) < 0)
                {
                    m_Min = m_MinLimit;
                    return;
                }
                if(value.CompareTo(m_Max) > 0)
                {
                    m_Max = value.CompareTo(m_MaxLimit) > 0 ? m_MaxLimit : value;
                    m_Min = m_Max;
                    return;
                }
                m_Min = value;
            }
        }

        public T Max
        {
            get => m_Max;

            set
            {
                if(EqualityComparer<T>.Default.Equals(m_Max, value)) return;

                if(value.CompareTo(m_MaxLimit) > 0)
                {
                    m_Max = m_MaxLimit;
                    return;
                }
                if(value.CompareTo(m_Min) < 0)
                {
                    m_Min = value.CompareTo(m_MinLimit) < 0 ? m_MinLimit : value;
                    m_Max = m_Min;
                    return;
                }
                m_Max = value;
            }
        }

        private readonly T m_MinLimit;
        private readonly T m_MaxLimit;

        public Range(T minLimit, T maxLimit, T min, T max)
        {
            Assert.IsTrue(minLimit.CompareTo(min) <= 0, "Min is less than MinLimit.");
            Assert.IsTrue(min.CompareTo(max) <= 0, "Min is greater than Max.");
            Assert.IsTrue(max.CompareTo(maxLimit) <= 0, "Max is greater than MaxLimit.");
            
            m_MinLimit = minLimit;
            m_MaxLimit = maxLimit;
            Min = min;
            Max = max;
        }

        public void Set(T min, T max)
        {
            Min = min;
            Max = max;
        }
    }

    public static class FloatRangeExtensions
    {
        public static float GetRandomValue(this Range<float> range, ref Random random)
        {
            return random.NextFloat(range.Min, range.Max);
        }

        public static Vector2 ToVector2(this Range<float> range)
        {
            return new Vector2(range.Min, range.Max);
        }

        public static void Set(this Range<float> range, Vector2 minMaxRange)
        {
            range.Set(minMaxRange.x, minMaxRange.y);
        }
    }

    public static class IntRangeExtensions
    {
        public static int GetRandomValue(this Range<int> range, ref Random random)
        {
            return random.NextInt(range.Min, range.Max + 1);
        }

        public static Vector2 ToVector2(this Range<int> range)
        {
            return new Vector2(range.Min, range.Max);
        }

        public static Vector2Int ToVector2Int(this Range<int> range)
        {
            return new Vector2Int(range.Min, range.Max);
        }

        public static void Set(this Range<int> range, Vector2Int minMaxRange)
        {
            range.Set(minMaxRange.x, minMaxRange.y);
        }
    }

    public static class UIntRangeExtensions
    {
        public static uint GetRandomValue(this Range<uint> range, ref Random random)
        {
            return random.NextUInt(range.Min, range.Max + 1);
        }

        public static Vector2 ToVector2(this Range<uint> range)
        {
            return new Vector2(range.Min, range.Max);
        }

        public static Vector2Int ToVector2Int(this Range<uint> range)
        {
            return new Vector2Int((int)range.Min, (int)range.Max);
        }

        public static void Set(this Range<uint> range, Vector2Int minMaxRange)
        {
            range.Set((uint)minMaxRange.x, (uint)minMaxRange.y);
        }
    }
}