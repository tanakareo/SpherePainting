using System;
using Random = Unity.Mathematics.Random;

public class ArrayShuffler
{
    // [start, end]の範囲の配列の要素をシャッフルする
    public static void ShuffleRange<T>(uint seed, T[] array, int start, int end)
    {
        Random random = new Random(seed);

        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (start < 0 || end >= array.Length || start > end)
            throw new ArgumentOutOfRangeException("start または end の値が不正です。");

        for (int i = end; i > start; i--)
        {
            int j = random.NextInt(start, i + 1);
            (array[i], array[j]) = (array[j], array[i]); // スワップ
        }
    }
}
