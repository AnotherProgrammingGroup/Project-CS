using System;

namespace ecube.the_10001st_prime {

    // PrimeGenerator class: generate the next prime number using our current list to speed up generation
    // primes[]: list of primes
    // size: max size of array of primes
    // end: last element in array
    // TODO: add support for BigInteger
    class PrimeGenerator {
        public int[] primes;
        public int size;
        public int end;
        public PrimeGenerator() {
            primes = new int[10001];
            size = 10001;
            end = 0;
            primes[0] = 2;
        }

        public PrimeGenerator(int maxSize) {
            size = maxSize;
            primes = new int[size];
            end = 0;
            primes[0] = 2;
        }
        
        // Generate next prime by using our current list of prime numbers.
        public int generateNext() {
            // Check if our generator is new.
            int next;
            if (end == 0) {
                next = 3;
            } else {
                next = primes[end];
            }

            // Iterate through the current prime numbers to see if our new factor is divisible by them.
            bool foundPrime = false;
            while (!foundPrime) {
                next += 2;
                bool isntPrime = true;
                for (int i = 0; i < end && primes[i] < next / 2 && isntPrime; i++) {
                    if (next % primes[i] == 0) {
                        // It's not a prime number, run away
                        isntPrime = false;
                    }
                }
                if (isntPrime) {
                    // We didn't find any prime factors, so it must be prime.
                    foundPrime = true;
                }
                
            }
            // Increment the end pointer.
            end++;
            primes[end] = next;
            return next;
        }
    }
    class Program {
        static int Main(string[] args) {

            PrimeGenerator gen = new PrimeGenerator(10002);
            for (int i = 0; i < 10001; i++) {
                //Console.Write("{0} ", gen.generateNext());
                gen.generateNext();
            }

            Console.WriteLine(gen.primes[10000]);
            //Console.ReadKey();
            return 0;
        }
    }
}
