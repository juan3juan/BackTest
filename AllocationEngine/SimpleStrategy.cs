using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllocationEngine
{
    public class SimpleStrategy : IStrategy
    {
        public Dictionary<string, int> ExecuteStrategy(double CurrentCash, Dictionary<string, int> CurrentPostions, Dictionary<string, double> BuyPrice, Dictionary<string, List<double>> CurrentPrice)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            int quantity = 0;
            string key = CurrentPrice.First().Key;
            int currentposition = 0;
            bool flagBuy = false;
            List<double> pricehistory = CurrentPrice[key];
            double currentPrice = pricehistory.Last();
            double previousPrice = pricehistory[pricehistory.Count-2];
            double Cash = CurrentCash;
            double buyPrice = 0;
            if (BuyPrice.ContainsKey(key))
            {
                currentposition = CurrentPostions[key];                
                buyPrice = BuyPrice[key];
                flagBuy = currentposition > 0 ? true : false;
            }

            #region logic to buy or sell stock
            if ((previousPrice > currentPrice * 1.03) && flagBuy == false)
            {
                flagBuy = true;
                quantity = (int)(Cash / currentPrice);
                result.Add(key, quantity);
            }
            else if (flagBuy == true)
            {
                if (currentPrice > buyPrice * 1.05)
                {
                    result.Add(key, -1*currentposition);
                }
            }

            if (result.Count==0)
            {
                result.Add(key, 0);
            }
            #endregion logic to buy or sell stock


            return result;
        }
    }
}
