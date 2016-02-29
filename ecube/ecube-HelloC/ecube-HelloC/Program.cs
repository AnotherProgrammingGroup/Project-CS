// HelloC program
// Project Euler Problem 5:
//   2520 is the smallest number that can be divided by each of the numbers from 1 to 10 without any remainder.
//   What is the smallest positive number that is evenly divisible by all of the numbers from 1 to 20?
//    The real result should be 232792560
// ecube


// TODO: smallest number that is evenly divisible by any two arbitrary numbers
// TODO: Split this file into multiple source files
using System;
using System.Numerics;
using System.Collections;

namespace ecube.HelloC {

    class Factors {
        // Stores prime numbers starting from 2
        public int[] primes;
        public int numPrimes;

        public int[] pused;

        public Factors(int num) {
            numPrimes = num;
            primes = new int[num];
        }


        // Arbitrary number 20
        // Don't use pl0x
        public Factors() {
            numPrimes = 20;
            primes = new int[] { 2, 3, 5, 7, 11,
                                 13, 17, 19, 23,
                                 29, 31, 37, 41,
                                 43, 47, 53, 59,
                                 61, 67, 71 };

            pused = new int[20];
        }

        // Sets numPrimes to 
        /*
        public void generatePrimes(int n) {
            numPrimes = n;
            primes = new int[n];
            pused = new int[n];

            for (int i = 0; i < n; i++) {
                for (int j = 2; j < i*i; j++) {
                    if (i % j != 0) {
                        wtf = false;

        }
        */

        public int findStoredPrime(int n) {
            return OperationsHQ.dumbSearchInt(primes, n, numPrimes);
        }

    }

    static class OperationsHQ {
        // Must be a pre-sorted array
        // Returns -1 on failure
        // TODO: Add some checks for small arrays
        // TODO: Actually implement later.  For now I'll just use stupid way
        public static int bSearchInt(int[] arr, int num, int size) {
            return 0;
            bool higher = false;
            bool someCondition = false;
            while (!someCondition) {

            }
            // We failed, run away!
            return -1;
        }

        // Best eaten sorted.
        // Precondition: array arr is sorted least to greatest. 
        public static int dumbSearchInt(int[] arr, int num, int size) {
            bool found = false;
            int i = 0;
            while (!found) {
                if (num < arr[i]) {
                    return -1;
                } else if (num == arr[i]) {
                    found = true;
                    return i;
                } else if (num > arr[i] && i < size) {
                    i++;
                } else {
                    return -1;
                }
            }
            // Something retarded happened, panic
            return -2;
        }
    }

    class Program {
        public static int Main(string[] args) {
            //BigInteger upper = new BigInteger(69);
            //BigInteger lower = new BigInteger(1337);
            int lower = 2;
            int upper = 20;
            // Parse arguments
            // TODO: actually parse argumentsa
            /*
            if (args.Length == 0) {
                upper = 2;
                lower = 20;
            } else {
                upper = 2;
                lower = 20;
            } */ // fken hell i'm stoopid

            // TODO: actually use lower and upper
            
            Factors allOfTheThings = new Factors();

            // i : [lower, upper) 
            // iterate through to find the LCM
            for (int i = lower; i < upper; i++) {
                int primeIndex = allOfTheThings.findStoredPrime(i);
                if (primeIndex >= 0) {
                    // TODO: Use private variable and public functions.

                    // If the number is already prime, we haven't seen it yet, so set times seen to 1.
                    allOfTheThings.pused[primeIndex] = 1;
                } else {
                    // Loop through the possible prime factors of i.
                    // j is our prime candidate. 
                    // Factor the number, and only add prime factor occurences that are bigger than what we've seen.
                    // TODO: We don't actually need to keep going until it's greater than i; add some division.
                    for (int j = 0; allOfTheThings.primes[j] < i; j++) {
                        // Make things easier to type.
                        int prm = allOfTheThings.primes[j];
                        if (i % prm == 0) {
                            // We found a prime factor, find it's maximum exponent
                            int powah = 1;
                            while (i % (int)Math.Pow(prm, powah) == 0) {
                                powah++;
                                
                                
                                if (powah > 9000 && false) Console.WriteLine("It's over 9000!");
                            }
                            // This power actually doesn't work, so go back to previous power
                            powah--;
                            
                            // Now write the power to our tracker array
                            if (powah > allOfTheThings.pused[j]) {
                                allOfTheThings.pused[j] = powah;
                            }
                                
                        } // if (j % prm == 0)
                    } // for (int j = 0....)
                } // if (primeIndex >= 0)
                
            } // for (int i = lower....)

            // We processed our things, print the result.
            BigInteger result = 1;
            for (int iter = 0; iter < allOfTheThings.numPrimes; iter++) {
                result *= (int)Math.Pow(allOfTheThings.primes[iter], allOfTheThings.pused[iter]);
                //Console.WriteLine("{0} {1} {2}", iter, allOfTheThings.primes[iter], allOfTheThings.pused[iter]);
            } // for (int iter = 0....)
            Console.WriteLine("The result is: {0}", result);
            

            // Let the humans read the result
            Console.ReadKey();
            return 0;
        } // public static int Main(string[] args)
    } // class Program
} // namespace ecube.HelloC

// EOF