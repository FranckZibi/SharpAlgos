using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpAlgos
{
    public static partial class Utils
    {
        //Compute max profit if only 1 transaction is allowed in o(n) time and o(1) memory
        //(we are allowed to have at most 1 auction in portfolio)
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

        //Compute max profit if we can do as many transactions as we wish in o(n) time and o(1) memory
        //(we are allowed to have at most 1 auction in portfolio)
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

        //Compute max profit if we can do as many transactions as we wish in o(n) time and o(1) memory
        //Here, we have the following restriction: if we sell a stock at date 'j', we can't buy one at 'j'+1'
        //(we are allowed to have at most 1 auction in portfolio)
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

        //Compute max profit if we can do at most K transactions in o(K*n) time and o(K) memory
        //(we are allowed to have at most 1 auction in portfolio)
        public static int MaxProfit_K_Transactions(int[] prices, int K, int transactionFee)
        {
            K = Math.Min(K, prices.Length / 2);
            var maxProfit_iTh_auction_InPortfolio = Enumerable.Repeat(int.MinValue / 2,1+K).ToArray();
            var maxProfitEmptyPortfolio_after_iTh_Transaction = Enumerable.Repeat(int.MinValue / 2, 1 + K).ToArray();
            maxProfitEmptyPortfolio_after_iTh_Transaction[0] = 0;
            var tmpMaxProfit_iTh_auction_InPortfolio = new int[1+K];
            var tmpMaxProfitEmptyPortfolio_after_iTh_Transaction = new int[1 + K];
            foreach (var p in prices)
            {
                for (int i = 1; i <= K; ++i)
                {
                    tmpMaxProfit_iTh_auction_InPortfolio[i] = Math.Max(maxProfit_iTh_auction_InPortfolio[i], maxProfitEmptyPortfolio_after_iTh_Transaction[i-1] - p);
                    tmpMaxProfitEmptyPortfolio_after_iTh_Transaction[i] = Math.Max(maxProfitEmptyPortfolio_after_iTh_Transaction[i], tmpMaxProfit_iTh_auction_InPortfolio[i] + p - transactionFee);
                }
                tmpMaxProfit_iTh_auction_InPortfolio.CopyTo(maxProfit_iTh_auction_InPortfolio, 0);
                tmpMaxProfitEmptyPortfolio_after_iTh_Transaction.CopyTo(maxProfitEmptyPortfolio_after_iTh_Transaction, 0);
            }
            return Math.Max(0, maxProfitEmptyPortfolio_after_iTh_Transaction.Max());
        }
    }
}