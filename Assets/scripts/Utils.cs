#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils {
    /// <summary>
    /// Returns the lowest common denominator between the numbers given.
    /// </summary>
    /// <param name="numbers">The numbers to check.</param>
    public static long LCM (long[] numbers) {
        return numbers.Aggregate(LCM);
    }

    /// <summary>
    /// Returns the lowest common denominator between two numbers.
    /// </summary>
    /// <param name="a">A number to check.</param>
    /// <param name="b">A number to check.</param>
    public static long LCM (long a, long b) {
        return System.Math.Abs(a * b) / GCD(a, b);
    }

    /// <summary>
    /// Returns the greatest common divisor between the numbers given.
    /// </summary>
    /// <param name="numbers">The numbers to check.</param>
    public static long GCD (long[] numbers) {
        return numbers.Aggregate(GCD);
    }

    /// <summary>
    /// Returns the greatest common divisor between two numbers.
    /// </summary>
    /// <param name="a">A number to check.</param>
    /// <param name="b">A number to check.</param>
    public static long GCD (long a, long b) {
        return b == 0 ? a : GCD(b, a % b);
    }

    /// <summary>
    /// Returns a random number between the bounds given. Note that the min
    /// value must be lower than the max value given, else it's undefined
    /// behavior.
    /// </summary>
    /// <param name="min">The lower bound (inclusive)</param>
    /// <param name="max">The upper bound (exclusive)</param>
    /// <returns></returns>
    public static long __DO_NOT_USE_RandomLong (long min = 0, long max = long.MaxValue) {
        // TODO: This is incredibly slow.
        long delta = max - min;

        int a = Random.Range(int.MinValue, int.MaxValue);
        int b = Random.Range(int.MinValue, int.MaxValue);

        long result = a + (b << 32);

        while (result < min) {
            result += delta;
        }

        result %= max;

        return result;
    }
}
