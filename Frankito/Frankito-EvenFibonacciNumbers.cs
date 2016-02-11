using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Program
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Two dos = new Two();
            Console.WriteLine(dos.Fibonacci());
        }
    }
    public class Two
    {
        public int Fibonacci()
        {
            List<int> num = new List<int>();
            num.Add(1);
            num.Add(2);
            int x = 0;
            int y = 0;
            while (num[x]<4000000)
            {
                num.Add(num[x]+num[x+1]);
                if (num[x] % 2 == 0)
                {
                    y += num[x];
                }
                ++x;
            }
            return y;
        }
    }
}
