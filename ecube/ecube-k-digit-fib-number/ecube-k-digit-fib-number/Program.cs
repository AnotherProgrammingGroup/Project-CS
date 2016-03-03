// Project Euler problem 25 (https://projecteuler.net/problem=25):
/*
The Fibonacci sequence is defined by the recurrence relation:

Fn = Fn−1 + Fn−2, where F1 = 1 and F2 = 1.
Hence the first 12 terms will be:

F1 = 1
F2 = 1
F3 = 2
F4 = 3
F5 = 5
F6 = 8
F7 = 13
F8 = 21
F9 = 34
F10 = 55
F11 = 89
F12 = 144
The 12th term, F12, is the first term to contain three digits.

What is the index of the first term in the Fibonacci sequence to contain 1000 digits?
*/
// ecube

using System;
using System.Numerics;

namespace ecube.k_digit_fib_number {
    class Program {
        static void Main(string[] args) {
            // res = n1 + n2
            BigInteger n1 = 1;
            BigInteger n2 = 1;
            BigInteger res = 2;
          

            // Index of res (fib number)
            int index = 3;

            // Keep finding next fibonacchi term until our fib number (result) has more than 1k digits
            // Remember, a number n has log10(n)+1 digits
            while (BigInteger.Log10(res) < 999) {
                n1 = n2;
                n2 = res;
                res = n1 + n2;
                index++;
            }

            Console.WriteLine(index);

        }
    }
}
