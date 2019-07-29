﻿using System.Collections.Generic;
// ReSharper disable UnusedMember.Global

namespace SharpAlgos
{
    public static partial class Utils
    {

        //compute n! in o(n) time (and o(1) memory)
        public static int Factorial(int n)
        {
            var result = 1;
            while (n >= 1)
            {
                result = result * n--;
            }
            return result;
        }

        public static void Shuffle<T>(IList<T> list, System.Random r)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                int k = r.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }


        //in a grid, we want to compute the number of paths from (x1,y1) to (x2,y2)  (with x2>=x1&&y2>=y1) 
        //(*) If we can go only right or up
        // => number of paths = C(x2-x1+y2-y1,x2-x1)  
        //(*) If we also can not cross the line y=x-c (<=> (x-y) must be always >= c)
        // => number of paths = C(x2-x1+y2-y1,x2-x1) - C(x2-x1+y2-y1,x2-y1-c+1)
        //special case : (x1,y1)=(0,0)  && (x2,y2)=(n,n)&& (c==0)
        // => number of paths = Catalan numbers = C(2n,n) - C(2n,n+1)

        //compute C(n,p) in o(n) time (and o(1) memory)
        public static int Combination(int n, int p)
        {
            if (p == 0 || n == p)
            {
                return 1;
            }
            if (p > n)
            {
                return 0;
            }
            var result = n;
            for (var i = 1; i < p; ++i)
            {
                result = (result*(n-i))/(i+1);
            }
            return result;
        }

        //compute the PGCD of 'a' & 'b' in o(?) time
        public static int PGCD(int a, int b)
        {
            var temp = a % b;
            return temp == 0 ? b : PGCD(b, temp);
        }

        // compute 'a^power%modulo' in o(log(power)) time
        public static long PowerModulo(long a, long power, long modulo)
        {
            if (power == 0)
            {
                return 1L;
            }
            var r = PowerModulo(a, power / 2, modulo);
            r = (r * r) % modulo;
            return (power % 2 == 0) ? r : (a * r) % modulo;
        }

        // compute ' (aNotMultipleOfPrimeModulo ^ power) % primeModulo' in o(log(min(power,primeModulo))) time
        //Only works if aNotMultipleOfPrimeModulo is not a multiple of primeModulo
        //Uses Euler Thoerem who states that if a & m are coprime, then:
        //  (a^power)%m = a^(power % EulerTotient(m)) % m
        //if m is prime, then EulerTotient(m) = m-1 and (if a is not a multiple of primeModulo) :
        //  (a^power)%primeModulo = a^(power % (primeModulo-1)) % primeModulo
        public static long PowerWithPrimeModulo(long aNotMultipleOfPrimeModulo, long power, long primeModulo)
        {
            //the Euler's totient function of primeModulo = primeModulo-1
            return PowerModulo(aNotMultipleOfPrimeModulo, power % (primeModulo - 1), primeModulo);
        }


        //compute the multiplicative inverse of 'a' in o(log(modulo)) time
        //it is the number X verifying: (a*X)%modulo = 1 and it only exists if 'a' & 'modulo' are coprimes (<=> PGCD(a, modulo) = 1 )
        public static int ModularMultiplicativeInverse(int a, int modulo)
        {
            if (modulo == 1)
            {
                return 0;
            }
            var y = 0;
            var x = 1;
            var m = modulo;
            while (a > 1)
            {
                var q = a / m;
                var t = m;
                m = a % m;
                a = t;
                t = y;
                y = x - q * y;
                x = t;
            }
            if (x < 0)
            {
                x += modulo;
            }
            return x;
        }

        //compute C(n,p) % primeModulo in o(p*log(primeModulo) time and o(1) memory
        //requirement: primeModulo is a prime > n
        public static int Combination_with_PrimeModulo(int n, int p, int primeModulo)
        {
            if (p == 0 || n == p)
            {
                return 1;
            }
            if (p > n)
            {
                return 0;
            }
            long result = n;
            for (var i = 1; i < p; ++i)
            {
                result = (result * (n - i)) % primeModulo;
                result = (result * ModularMultiplicativeInverse(i + 1, primeModulo)) % primeModulo;
            }
            return (int)result;
        }

        //compute C(n,p) % primeModulo in o(1) time
        //requirement: 'primeModulo is a prime > n
        //needs a precomputation that takes o(n) time and o(n) memory (the 2 arrays in parameters):
        //      var factorialsModulo_up_to_n = FactorialsModulo(n, primeModulo);
        //      var factorialsModularMultiplicativeInverse_up_to_n = FactorialsModularMultiplicativeInverse(factorialsModulo_up_to_n, primeModulo);
        public static int Combination_with_PrimeModulo(int n, int p, int primeModulo, int[] factorialsModulo_up_to_n, int[] factorialsModularMultiplicativeInverse_up_to_n)
        {
            if (p == 0 || n == p)
            {
                return 1;
            }
            if (p > n)
            {
                return 0;
            }
            long result = factorialsModulo_up_to_n[n];
            result = (result * factorialsModularMultiplicativeInverse_up_to_n[p]) % primeModulo;
            result = (result * factorialsModularMultiplicativeInverse_up_to_n[n - p]) % primeModulo;
            return (int)result;
        }

        //compute 'i! % modulo' for all 'i' in [0,n] in o(n) time and o(n) memory
        // factorialModulo[i] = i! % modulo
        public static int[] FactorialsModulo(int n, int modulo)
        {
            var factorialModulo = new int[1 + n];
            factorialModulo[0] = 1;
            for (var i = 1; i <= n; ++i)
            {
                factorialModulo[i] = (int)((((long)i) * factorialModulo[i - 1]) % modulo);
            }
            return factorialModulo;
        }


        //returns the multiplicative inverse of all factoriels between 0 and n (=alreadyComputedFactorielModulo.Length) in o(n) time and o(n) memory
        //  (n! * multiplicativeInverse[n]) % primeModulo = 1
        //requirement: 'primeModulo' is a prime > n
        public static int[] FactorialsModularMultiplicativeInverse(int[] alreadyComputedFactorialModulo, int primeModulo)
        {
            var modularMultiplicativeInverses = new int[alreadyComputedFactorialModulo.Length];
            var n = alreadyComputedFactorialModulo.Length - 1;
            modularMultiplicativeInverses[0] = 1;
            modularMultiplicativeInverses[n] = ModularMultiplicativeInverse(alreadyComputedFactorialModulo[n], primeModulo);
            for (var i = n - 1; i >= 1; --i)
            {
                modularMultiplicativeInverses[i] = (int)((modularMultiplicativeInverses[i + 1] * (long)(i + 1)) % primeModulo);
            }
            return modularMultiplicativeInverses;
        }

        #region prime numbers detection
        //detect if 'n' is prime in o(sqrt(n)) time
        public static bool IsPrime(int n)
        {
            if (n <= 1)
            {
                return false;
            }
            if ((n % 2) == 0)
            {
                return n == 2;
            }
            for (var divider = 3; divider * divider <= n; divider += 2)
            {
                if (n % divider == 0)
                {
                    return false;
                }
            }
            return true;
        }
        //retrieve all primes between 0 and 'n' in o(n log(log(n)) ) time (and o(n) memory) using sieve of eratosthenis
        public static bool[] AllPrimes(int n)
        {
            var primes = new bool[1 + n];
            for (var i = 2; i < primes.Length; ++i)
            {
                primes[i] = true;
            }
            for (var i = 2; i <= n; ++i)
            {
                if (primes[i]) //if 'i' is prime , we discard all multiples of 'i'
                {
                    for (var j = 2 * i; j <= n; j += i)
                    {
                        primes[j] = false;
                    }
                }
            }
            return primes;
        }
        //TODO add tests
        public static List<int> PrimeFactorization(int n)
        {
            var result = new List<int>();
            while (n % 2 == 0)
            {
                result.Add(2);
                n /= 2;
            }
            for (var i = 3; i * i <= n; i += 2)
            {
                while (n % i == 0)
                {
                    result.Add(i);
                    n /= i;
                }
            }
            if (n > 2)
            {
                result.Add(n);
            }
            return result;
        }

        #endregion

    }
}