using System;
					
public class Program
{
	public static void Main()
	{
		Random random = new Random();
		int heads = 0;
		int tails = 0;
		for (int i = 0; i<10000000; ++i)
		{
			int roll = random.Next(2);
			if (roll == 0)
			{
				++heads;
			}
			else
			{
				++tails;
			}
		}
		Console.WriteLine("Heads: {0}", heads);
		Console.WriteLine("Tails: {0}", tails);
		if ((heads/1000000) % 4 == 0 || (tails/1000000) % 4 == 0)
		{
			heads = (heads/1000000)/4;
			tails = (tails/1000000)/4;
		}
		else if ((heads/1000000) % 5 == 0 || (tails/1000000) % 5 == 0)
		{
			heads = (heads/1000000)/5;
			tails = (tails/1000000)/5;
		}
		Console.WriteLine("Ratio: {0}:{0}", heads, tails);			  
	}
}
