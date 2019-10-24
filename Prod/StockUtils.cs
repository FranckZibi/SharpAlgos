using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpAlgos
{
    public static partial class Utils
    {
        /// <summary>
        /// Compute max profit if only 1 transaction is allowed (we are allowed to have at most 1 auction in portfolio)
        /// Complexity:         o( n )
        /// Memory Complexity:  o( 1 )
        /// </summary>
        /// <param name="prices"></param>
        /// <param name="transactionFee"></param>
        /// <returns></returns>
        public static int MaxProfit_1Transaction(IEnumerable<int> prices, int transactionFee)
        {
            int minPrice = int.MaxValue/2;
            int maxProfit = 0;
            foreach (var p in prices)
            {
                maxProfit = Math.Max(maxProfit, p-minPrice-transactionFee);
                minPrice = Math.Min(minPrice, p);
            }
            return maxProfit;
        }

        /// <summary>
        /// Compute max profit if we can do as many transactions as we wish (we are allowed to have at most 1 auction in portfolio)
        /// Complexity:         o( n )
        /// Memory Complexity:  o( 1 )
        /// </summary>
        /// <param name="prices"></param>
        /// <param name="transactionFee"></param>
        /// <returns></returns>
        public static int MaxProfit_InfiniteTransaction(IEnumerable<int> prices, int transactionFee)
        {
            int maxProfitAuctionInPortfolio = int.MinValue / 2;
            int maxProfitEmptyPortfolio = 0;
            foreach (var p in prices)
            {
                var tmpMaxProfitAuctionInPortfolio = Math.Max(maxProfitAuctionInPortfolio, maxProfitEmptyPortfolio - p);
                var tmpMaxProfitEmptyPortfolio = Math.Max(maxProfitEmptyPortfolio, maxProfitAuctionInPortfolio + p-transactionFee);
                maxProfitAuctionInPortfolio = tmpMaxProfitAuctionInPortfolio;
                maxProfitEmptyPortfolio = tmpMaxProfitEmptyPortfolio;
            }
            return maxProfitEmptyPortfolio;
        }

        /// <summary>
        /// Compute max profit if we can do as many transactions as we wish (we are allowed to have at most 1 auction in portfolio)
        /// Here, we have the following restriction: if we sell a stock at date 'j', we can't buy one at 'j'+1'
        /// Complexity:         o( n )
        /// Memory Complexity:  o( 1 )
        /// </summary>
        /// <param name="prices"></param>
        /// <param name="transactionFee"></param>
        /// <returns></returns>
        public static int MaxProfit_InfiniteTransaction_OneDayFreezeAfterSellingStock(IEnumerable<int> prices, int transactionFee)
        {
            int maxProfitAuctionInPortfolio = int.MinValue / 2;
            int maxProfitEmptyPortfolioBecauseJustSold = 0;
            int maxProfitEmptyPortfolioWasAlreadyEmpty = 0;
            foreach (var p in prices)
            {
                var tmpMaxProfitAuctionInPortfolio = Math.Max(maxProfitAuctionInPortfolio, maxProfitEmptyPortfolioWasAlreadyEmpty - p);
                var tmpMaxProfitEmptyPortfolioBecauseJustSold = Math.Max(maxProfitEmptyPortfolioBecauseJustSold, maxProfitAuctionInPortfolio + p - transactionFee);
                var tmpMaxProfitEmptyPortfolioWasAlreadyEmpty = Math.Max(maxProfitEmptyPortfolioWasAlreadyEmpty, maxProfitEmptyPortfolioBecauseJustSold);
                maxProfitAuctionInPortfolio = tmpMaxProfitAuctionInPortfolio;
                maxProfitEmptyPortfolioBecauseJustSold = tmpMaxProfitEmptyPortfolioBecauseJustSold;
                maxProfitEmptyPortfolioWasAlreadyEmpty = tmpMaxProfitEmptyPortfolioWasAlreadyEmpty;
            }
            return Math.Max(maxProfitEmptyPortfolioBecauseJustSold, maxProfitEmptyPortfolioWasAlreadyEmpty);
        }

        /// <summary>
        /// Compute max profit if we can do at most K transactions (we are allowed to have at most 1 auction in portfolio)
        /// Complexity:         o( K*n )
        /// Memory Complexity:  o( K )
        /// </summary>
        /// <param name="prices"></param>
        /// <param name="k"></param>
        /// <param name="transactionFee"></param>
        /// <returns></returns>
        public static int MaxProfit_K_Transactions(int[] prices, int k, int transactionFee)
        {
            k = Math.Min(k, prices.Length / 2);
            var maxProfitIThAuctionInPortfolio = Enumerable.Repeat(int.MinValue / 2,1+k).ToArray();
            var maxProfitEmptyPortfolioAfterIThTransaction = Enumerable.Repeat(int.MinValue / 2, 1 + k).ToArray();
            maxProfitEmptyPortfolioAfterIThTransaction[0] = 0;
            var tmpMaxProfitIThAuctionInPortfolio = new int[1+k];
            var tmpMaxProfitEmptyPortfolioAfterIThTransaction = new int[1 + k];
            foreach (var p in prices)
            {
                for (int i = 1; i <= k; ++i)
                {
                    tmpMaxProfitIThAuctionInPortfolio[i] = Math.Max(maxProfitIThAuctionInPortfolio[i], maxProfitEmptyPortfolioAfterIThTransaction[i-1] - p);
                    tmpMaxProfitEmptyPortfolioAfterIThTransaction[i] = Math.Max(maxProfitEmptyPortfolioAfterIThTransaction[i], tmpMaxProfitIThAuctionInPortfolio[i] + p - transactionFee);
                }
                tmpMaxProfitIThAuctionInPortfolio.CopyTo(maxProfitIThAuctionInPortfolio, 0);
                tmpMaxProfitEmptyPortfolioAfterIThTransaction.CopyTo(maxProfitEmptyPortfolioAfterIThTransaction, 0);
            }
            return Math.Max(0, maxProfitEmptyPortfolioAfterIThTransaction.Max());
        }
    }
}
