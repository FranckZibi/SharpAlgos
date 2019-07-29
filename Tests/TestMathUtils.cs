using System;
using System.Collections.Generic;
using System.Linq;
using SharpAlgos;
using NUnit.Framework;

namespace SharpAlgosTests
{
    [TestFixture]
    public partial class TestUtils
    {
        [Test]
        public void TestFactorialsModulo()
        {
            var modulo = 11;
            var n = 10;
            var result = Utils.FactorialsModulo(n, modulo);
            Assert.AreEqual(1 + n, result.Length);
            for (int i = 0; i <= n; ++i)
                Assert.AreEqual(Utils.Factorial(i) % modulo, result[i]);
        }




        [Test]
        public void TestFactorialsModularMultiplicativeInverse()
        {
            var n = 10;
            var primeModulo = 11;
            var result = Utils.FactorialsModularMultiplicativeInverse(Utils.FactorialsModulo(n, primeModulo), primeModulo);
            Assert.AreEqual(1 + n, result.Length);
            for (int i = 0; i <= n; ++i)
                Assert.AreEqual(Utils.ModularMultiplicativeInverse(FactorialModulo(i, primeModulo), primeModulo), result[i]);
            n = 1000;
            primeModulo = (int)(1e9 + 7);
            result = Utils.FactorialsModularMultiplicativeInverse(Utils.FactorialsModulo(n, primeModulo), primeModulo);
            Assert.AreEqual(1 + n, result.Length);
            for (int i = n; i >= 0; --i)
                Assert.AreEqual(Utils.ModularMultiplicativeInverse(FactorialModulo(i, primeModulo), primeModulo), result[i]);

            n = 1000000;
            primeModulo = (int)(1e9 + 7);
            var factorialsWithPrimeModulo = Utils.FactorialsModulo(n, primeModulo);
            var factorialsModularMultiplicativeInverse = Utils.FactorialsModularMultiplicativeInverse(factorialsWithPrimeModulo, primeModulo);
            Assert.AreEqual(1 + n, factorialsModularMultiplicativeInverse.Length);
            for (int i = n; i >= 0; --i)
                Assert.AreEqual(1, (factorialsWithPrimeModulo[i] * ((long)factorialsModularMultiplicativeInverse[i])) % primeModulo);
        }

        private static int FactorialModulo(int n, int modulo)
        {
            long factorialModulo = 1;
            for (int i = 1; i <= n; ++i)
                factorialModulo = (i * factorialModulo) % modulo;
            return (int)factorialModulo;
        }

        [TestCase(1, 5, 0, 7)]
        [TestCase(0, 10, (int)1e9, 5)]
        [TestCase(6, (int)(1e9 + 7), (int)(1e9 + 7), 7)]
        public void TestPowerModulo(int expectedResult, int a, int power, int modulo)
        {
            Assert.AreEqual(expectedResult, Utils.PowerModulo(a, power, modulo));
        }

        [TestCase(1, 5, 0, 7)]
        [TestCase(6, (int)(1e9 + 7), (int)(1e9 + 7), 7)]
        public void TestPowerWithPrimeModulo(int expectedResult, int a, int power, int primeModulo)
        {
            Assert.AreEqual(expectedResult, Utils.PowerWithPrimeModulo(a, power, primeModulo));
        }
        

        [TestCase(5, 5, 5)]
        [TestCase(7, 7, 21)]
        [TestCase(1, 17, (int)(1e9 + 7))]
        [TestCase(2, 124578, 57848)]
        public void TestPGCD(int expectedResult, int a, int b)
        {
            Assert.AreEqual(expectedResult, Utils.PGCD(a, b));
        }


        [TestCase(5, 3, 7)]
        [TestCase(4, 3, 11)]
        [TestCase(12, 10, 17)]
        [TestCase(414770, 2018, 1000007)]
        [TestCase(124982126, 1000007, (int)(1e9 + 7))]
        public void TestModularMultiplicativeInverse(int expectedModularMultiplicativeInverse, int a, int modulo)
        {
            Assert.AreEqual(expectedModularMultiplicativeInverse, Utils.ModularMultiplicativeInverse(a, modulo));
        }

        [TestCase(100, 1, 100)]
        [TestCase(100, 2, 4950)]
        [TestCase(5, 3, 10)]
        [TestCase(13, 7, 1716)]
        [TestCase(12, 12, 1)]
        [TestCase(0, 0, 1)]
        [TestCase(1000, 0, 1)]
        [TestCase(0, 1, 0)]
        [TestCase(1, 2, 0)]
        [TestCase(1000, 2000, 0)]
        public void TestCombination(int n, int p, int expected)
        {
            Assert.AreEqual(expected, Utils.Combination(n, p));
        }


        [TestCase(100, 2, 1009)]
        [TestCase(15, 5, 17)]
        [TestCase(13, 7, 17)]
        [TestCase(7, 3, 11)]
        [TestCase(11, 11, 13)]
        [TestCase(0, 0, 1009)]
        [TestCase(1000, 0, 1009)]
        [TestCase(0, 1, 1009)]
        [TestCase(1, 2, 1009)]
        [TestCase(1000, 2000, 1009)]
        public void TestCombination_with_PrimeModulo(int n, int p, int primeModulo)
        {
            var expected = Utils.Combination(n, p) % primeModulo;
            Assert.AreEqual(expected, Utils.Combination_with_PrimeModulo(n, p, primeModulo));
            var factorials_with_PrimeModulo_up_to_n = Utils.FactorialsModulo(n, primeModulo);
            var factorialsModularMultiplicativeInverse_up_to_n = Utils.FactorialsModularMultiplicativeInverse(factorials_with_PrimeModulo_up_to_n, primeModulo);
            Assert.AreEqual(expected, Utils.Combination_with_PrimeModulo(n, p, primeModulo, factorials_with_PrimeModulo_up_to_n, factorialsModularMultiplicativeInverse_up_to_n));
        }

      
        [Test]
        public void TestPrimeDetection()
        {
            var primes = Utils.AllPrimes(10000);
            for (int i = 0; i < primes.Length; ++i)
                Assert.AreEqual(Utils.IsPrime(i), primes[i]);
        }


    }
}
