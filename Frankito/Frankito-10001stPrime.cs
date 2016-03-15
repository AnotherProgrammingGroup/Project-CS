using System;
using System.Collections.Generic;
using System.Linq;
					
public class Program
{
	public static void Main()
	{
		List<int> primes = new List<int>();
		primes.Add(2);
		primes.Add(3);
		int num = 5;
		while (primes.Count() < 10002)
		{
			bool prime = true;
			for (int i = 0; i < primes.Count(); ++i)
			{
				if (num % primes[i] == 0)
				{
					prime = false;
				}
			}
			if (prime == true)
			{
				primes.Add(num);
			}
			++num;
		}
		Console.WriteLine(primes[10000]);
	}
}
