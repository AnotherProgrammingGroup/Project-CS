using System;
using System.Diagnostics;

namespace CaveStory.CountingTuples
{
    static class Solution
    {
        public static void Main()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Console.WriteLine("T(3, 3) = " + solve(3, 3));
            Console.WriteLine("T(10, 10) = " + solve(10, 10));
            Console.WriteLine("T(1000, 1000) = " + solve(1000, 1000));
            Console.WriteLine("T(20000, 20000) = " + solve(20000, 20000));

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }

        private static long solve(int k, int n)
        {
            long[] freq = new long[n + 1];
            int x = 1, pi = 0;
            while (true)
            {
                if (isPrime(x))
                {
                    ++pi;
                }
                if (pi > n)
                {
                    break;
                }
                ++freq[pi];
                ++x;
            }
            long[] cur = new long[n + 1];
            cur[0] = 1;
            for (int i = 30; i >= 0; i--) {
                long[] tmp = 
                    NumberTheoreticTransform.convolve(ref cur, ref cur);
                Array.Copy(tmp, cur, n + 1);
                if (((k >> i) & 1) == 1)
                {
                    tmp = NumberTheoreticTransform.convolve(ref cur, ref freq);
                    Array.Copy(tmp, cur, n + 1);
                }
            }
            return cur[n];
        }

        private static bool isPrime(int x)
        {
            if (x <= 1)
            {
                return false;
            }
            for (int d = 2; d * d <= x; d++)
            {
                if (x % d == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }

    static class NumberTheoreticTransform
    {
        static readonly long mod = 1004535809;
        static readonly long i2 = 502267905;
        static readonly long primitive = 702606812;
        static readonly long inverse = 700146880;
        static readonly int order = 21;
        static readonly long[] primitives, inverses;

        static NumberTheoreticTransform()
        {
            primitives = new long[order];
            primitives[0] = primitive;
            for (int i = 1; i < order; i++)
            {
                primitives[i] = primitives[i - 1] * primitives[i - 1] % mod;
            }
            inverses = new long[order];
            inverses[0] = inverse;
            for (int i = 1; i < order; i++)
            {
                inverses[i] = inverses[i - 1] * inverses[i - 1] % mod;
            }
        }

        public static int log(int x)
        {
            int ret = 0;
            while (x > 1)
            {
                x /= 2;
                ++ret;
            }
            return ret;
        }

        public static void transform(ref long[] a)
        {
            int n = a.Length;
            if (n == 1)
            {
                return;
            }

            long[] a0 = new long[n / 2], a1 = new long[n / 2];
            for (int i = 0; i < n / 2; i++)
            {
                a0[i] = a[2 * i];
                a1[i] = a[2 * i + 1];
            }
            transform(ref a0);
            transform(ref a1);

            long root = primitives[order - log(n)], cur = 1;
            for (int i = 0; i < n; i++)
            {
                a[i] = (a0[i % (n / 2)] + cur * a1[i % (n / 2)]) % mod;
                cur = cur * root % mod;
            }
        }

        public static void invert(ref long[] a)
        {
            int n = a.Length;
            if (n == 1)
            {
                return;
            }

            long[] a0 = new long[n / 2], a1 = new long[n / 2];
            for (int i = 0; i < n / 2; i++)
            {
                a0[i] = a[2 * i];
                a1[i] = a[2 * i + 1];
            }
            invert(ref a0);
            invert(ref a1);

            long root = inverses[order - log(n)], cur = 1;
            for (int i = 0; i < n; i++)
            {
                a[i] = (a0[i % (n / 2)] + cur * a1[i % (n / 2)]) % mod;
                a[i] = i2 * a[i] % mod;
                cur = cur * root % mod;
            }
        }

        public static long[] convolve(ref long[] a, ref long[] b)
        {
            int n = 1;
            while (n < a.Length)
            {
                n *= 2;
            }
            n *= 2;

            long[] ta = new long[n], tb = new long[n];
            Array.Copy(a, ta, a.Length);
            Array.Copy(b, tb, b.Length);

            transform(ref ta);
            transform(ref tb);
            for (int i = 0; i < n; i++)
            {
                ta[i] = ta[i] * tb[i] % mod;
            }
            invert(ref ta);

            return ta;
        }
    }
}
