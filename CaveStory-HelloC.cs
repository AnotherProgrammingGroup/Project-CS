using System;
using System.Numerics;

namespace CaveStory.HelloCS
{
    class MainApp
    {
        public static void Main()
        {
            GeneralFibbonacci standardFib = new GeneralFibbonacci();
            Console.WriteLine("Standard Fibbonacci: ");
            for (int i = 1; i <= 10; i++)
            {
                Console.Write(standardFib.get(i) + " ");
            }
            Console.WriteLine("... {0} (42nd value) ...", standardFib.get(42));
            GeneralFibbonacci sexyFib = new GeneralFibbonacci(6, 9);
            Console.WriteLine("Sexy Fibbonacci: ");
            for (int i = 1; i <= 10; i++)
            {
                Console.Write(sexyFib.get(i) + " ");
            }
            Console.WriteLine("... {0} (42nd value) ...", sexyFib.get(42));
            Console.WriteLine("Element zero of the sequence is undefined: {0}"
                , sexyFib.get(0));
            Console.WriteLine("And we can go arbitrarily big " + 
                "(1337th element of sexy fibonacci):\n{0}", sexyFib.get(1337));
        }
    }

    class GeneralFibbonacci
    {
        private BigInteger a1, a2;

        public GeneralFibbonacci(int _a1, int _a2)
        {
            a1 = new BigInteger(_a1);
            a2 = new BigInteger(_a2);
        }

        public GeneralFibbonacci()
            : this(0, 1)
        {
        }

        //Get nth fibbonacci number 0-indexed.
        public BigInteger get(int n)
        {
            if (n <= 0)
            {
                return new BigInteger(-1);
            }
            else if (n == 1)
            {
                return a1;
            }
            else if (n == 2)
            {
                return a2;
            }
            else 
            {
                BigInteger prv = a1, cur = a2;
                for (int i = 2; i <= n; i++)
                {
                    BigInteger tmp = cur;
                    cur += prv;
                    prv = tmp;
                }
                return cur;
            }
        }
    }
}
