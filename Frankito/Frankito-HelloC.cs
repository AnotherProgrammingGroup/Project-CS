using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HelloC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Rectangle square = new Rectangle();
            square.Area(4, 4);
            Triangle right = new Triangle();
            right.Area(4, 4);
            Loop FB = new Loop();
            FB.fizzBuzz();
        }
    }
    public class Rectangle
    {
        public void Area(double length, double width)
        {
            Console.WriteLine("Square:");
            Console.WriteLine("Length: {0}", length);
            Console.WriteLine("Width: {0}", width);
            Console.WriteLine("Area: {0}", (length*width));
            Console.WriteLine();
        }
    }
    public class Triangle
    {
        public void Area(double length, double width)
        {
            Console.WriteLine("Triangle:");
            Console.WriteLine("Length: {0}", length);
            Console.WriteLine("Width: {0}", width);
            Console.WriteLine("Area: {0}", ((length*width)/2));
        }
    }
    public class Loop
    {
        public void fizzBuzz()
        {
            for (int i = 1; i<21; ++i)
            {
                if (i%3 == 0 && i%5 == 0)
                {
                    Console.WriteLine("FizzBuzz");
                }
                else if (i%3 == 0)
                {
                    Console.WriteLine("Fizz");
                }
                else if (i%5 == 0)
                {
                    Console.WriteLine("Buzz");
                }
                else
                {
                    Console.WriteLine(i);
                }
            }
        }
    }
}
