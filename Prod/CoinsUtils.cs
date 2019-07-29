using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpAlgos
{
    public static partial class Utils
    {
        //returns the minimum number of coins (among 'coinsWithDuplicates') to make exactly amount 'amount'  in o(amount*coins.Length) time
        //returns -1 if there is no way to achieve 'amount'
        //'coinsWithDuplicates' is the list of available coins, and may include duplicates
        public static int MinimumCoinChangeMakingWithDuplicateCoins(int[] coinsWithDuplicates, int amount)
        {
            return MinimumCoinChangeMakingWithDuplicateCoins_Helper(coinsWithDuplicates, coinsWithDuplicates.Length, amount, new int?[1 + amount, 1 + coinsWithDuplicates.Length]);
        }
        private static int MinimumCoinChangeMakingWithDuplicateCoins_Helper(int[] coinsWithDuplicates, int nbAllowedCoins, int remainingAmount, int?[,] cache)
        {
            if (remainingAmount == 0)
            {
                return 0;
            }
            if (remainingAmount < 0||nbAllowedCoins <= 0)
            {
                return -1;
            }
            if (cache[remainingAmount, nbAllowedCoins].HasValue)
            {
                return cache[remainingAmount, nbAllowedCoins].Value;
            }
            int usingLastAllowedCoin = MinimumCoinChangeMakingWithDuplicateCoins_Helper(coinsWithDuplicates, nbAllowedCoins - 1, remainingAmount - coinsWithDuplicates[nbAllowedCoins - 1], cache);
            if (usingLastAllowedCoin != -1)
            {
                ++usingLastAllowedCoin;
            }
            int notUsingLastAllowedCoin = MinimumCoinChangeMakingWithDuplicateCoins_Helper(coinsWithDuplicates, nbAllowedCoins - 1, remainingAmount, cache);
            int result = usingLastAllowedCoin;
            if (usingLastAllowedCoin == -1 || (notUsingLastAllowedCoin != -1 && notUsingLastAllowedCoin < usingLastAllowedCoin))
            {
                result = notUsingLastAllowedCoin;
            }
            cache[remainingAmount, nbAllowedCoins] = result;
            return result;
        }


        //return the minimum number of coins needed to achieve exactly 'amount'
        // (return -1 if it is not possible)
        public static int CoinChange(int[] coins, int amount)
        {
            return CoinChange_Helper(coins, amount, new Dictionary<int, int>());
        }
        public static int CoinChange_Helper(int[] coins, int remainingAmount, Dictionary<int, int> cache)
        {
            if (remainingAmount == 0)
            {
                return 0;
            }
            if (remainingAmount < 0)
            {
                return -1;
            }

            if (cache.ContainsKey(remainingAmount))
            {
                return cache[remainingAmount];
            }

            int currentMin = int.MaxValue;
            foreach (var face in coins)
            {
                int result = CoinChange_Helper(coins, remainingAmount - face, cache);
                if (result != -1)
                {
                    currentMin = Math.Min(currentMin, 1 + result);
                }
            }
            if (currentMin == int.MaxValue)
            {
                currentMin = -1;
            }
            cache[remainingAmount] = currentMin;
            return currentMin;
        }

        //count the number of ways to achieve 'amount' given an infinite number of coins of face 'coins'
        //for instance, there are 3 ways to achieve 4 cents with coins of 1cent and 2cents (2*2cents, 4*1cent, 2*1cent+2cent)
        public static int CountWaysToAchieveAmount(int amount, int[] coins)
        {
            var prev = new int[1 + amount];
            var current = new int[1 + amount];
            prev[0] = 1;

            for (int i = 0; i < coins.Length; ++i)
            {
                current[0] = 1;
                for (int a = 1; a <= amount; ++a)
                {
                    int nbWithoutCoinI = prev[a];
                    int nbWithCoinI = (a >= coins[i]) ? current[a - coins[i]] : 0;
                    current[a] = nbWithoutCoinI + nbWithCoinI;
                }
                var tmp = current;
                current = prev;
                prev = tmp;
            }
            return prev.Last();
        }

        public static int TotalNumberOfWaysToGetTheDenominationOfCoins(int target, int[] values)
        {
            var hashValues = new HashSet<int>(values);
            Array.Sort(values);
            int[] numberOfWays = new int[1 + target];
            numberOfWays[0] = 1;
            for (int targetValue = 1; targetValue < numberOfWays.Length; ++targetValue)
            {
                for (var leftVal = 1; leftVal < targetValue; leftVal++)
                {
                    numberOfWays[targetValue] = Math.Max(numberOfWays[targetValue], numberOfWays[leftVal] * numberOfWays[targetValue - leftVal]);
                }
                if (hashValues.Contains(targetValue))
                {
                    ++numberOfWays[targetValue];
                }
            }
            return numberOfWays[target];
        }


        //Solve minium Coin change-making problem (unlimited supply of coins) in o(target*values.Length) time
        public static int MinimumCoinChangeMaking(int target, int[] values)
        {
            Array.Sort(values);
            int[] minimumCoins = new int[1 + target];
            for (int targetValue = 1; targetValue < minimumCoins.Length; ++targetValue)
            {
                minimumCoins[targetValue] = int.MaxValue;
                foreach (var face in values)
                {
                    if (face > targetValue)
                    {
                        break;
                    }
                    if (minimumCoins[targetValue - face] != int.MaxValue)
                    {
                        minimumCoins[targetValue] = Math.Min(minimumCoins[targetValue], 1 + minimumCoins[targetValue - face]);
                    }
                }
            }
            if (minimumCoins[target] == int.MaxValue)
            {
                return -1; // not possible
            }
            return minimumCoins[target];
        }

    }
}