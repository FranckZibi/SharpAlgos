using System;
using System.Linq;

namespace SharpAlgos
{
    public static partial class Utils
    {
        /// <summary>
        /// Compute the minimum number of coins (among 'coinsWithDuplicates') to make exactly amount 'amount'
        /// Complexity:         o( amount * coinsWithDuplicates.Length)
        /// Memory Complexity:  o( amount * coinsWithDuplicates.Length)
        /// </summary>
        /// <param name="amount">amount to achieve</param>
        /// <param name="coinsWithDuplicates">list of available coins (some of them may have the same face value (duplicates)</param>
        /// <returns> minimum number of coins (among 'coinsWithDuplicates') to make exactly amount 'amount'
        /// -1 if there is no way to achieve 'amount'
        /// </returns>
        public static int MinimumCoinChangeMakingWithDuplicateCoins(int amount, int[] coinsWithDuplicates)
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

        /// <summary>
        /// Solve minimum Coin change-making problem to achieve exactly 'amount' given an infinite number of coins of face 'coins'
        /// Complexity:         o( amount * coins.Length)
        /// Memory Complexity:  o( amount)
        /// </summary>
        /// <param name="amount">the amount to achieve</param>
        /// <param name="coins">list of coin face values</param>
        /// <returns>
        /// minimum number of coins needed to achieve exactly 'amount'
        /// -1 if it is not possible
        /// </returns>
        public static int MinimumCoinChangeMaking(int amount, int[] coins)
        {
            Array.Sort(coins);
            int[] minimumCoins = new int[1 + amount];
            for (int targetValue = 1; targetValue < minimumCoins.Length; ++targetValue)
            {
                minimumCoins[targetValue] = int.MaxValue;
                foreach (var face in coins)
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
            if (minimumCoins[amount] == int.MaxValue)
            {
                return -1; // not possible
            }
            return minimumCoins[amount];
        }

        /// <summary>
        /// Count the number of ways to achieve 'amount' given an infinite number of coins of face 'coins'
        /// for instance, there are 3 ways to achieve 4 cents with coins of 1cent and 2cents (2*2cents, 4*1cent, 2*1cent+2cent)
        /// Complexity:         o( amount * coins.Length)
        /// Memory Complexity:  o( amount )
        /// </summary>
        /// <param name="amount">the amount to achieve</param>
        /// <param name="coins">list of coin face values</param>
        /// <returns>the number of ways to achieve the amount 'amount'</returns>
        public static int CountWaysToAchieveAmount(int amount, int[] coins)
        {
            var numberOfWays = new int[1 + amount];
            numberOfWays[0] = 1;
            var current = new int[1 + amount];
            
            for (int i = 0; i < coins.Length; ++i)
            {
                current[0] = 1;
                for (int a = 1; a <= amount; ++a)
                {
                    int nbWithoutCoinI = numberOfWays[a];
                    int nbWithCoinI = (a >= coins[i]) ? current[a - coins[i]] : 0;
                    current[a] = nbWithoutCoinI + nbWithCoinI;
                }
                var tmp = current;
                current = numberOfWays;
                numberOfWays = tmp;
            }
            return numberOfWays.Last();
        }
    }
}
