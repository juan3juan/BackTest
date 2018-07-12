using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace BackTest
{
    class BackTestBiz
    {
        public static void Run(Dictionary<string, List<PricingData>> timeseries, double capital)
        {
            List<PricingData> ps = timeseries["BABA"];
            List<Order> stockOrder = new List<Order>();

            bool flagBuy = false;
             
            for (int i=0; i<ps.Count-1; i++)
            {
                double previousPrice = ps[i].ClosePrice;
                double currentPrice = ps[i+1].ClosePrice;
                DateTime currentDate = ps[i + 1].Date;
                int quantity = 0;
                if ((previousPrice > currentPrice *1.03) && flagBuy == false)
                {
                    flagBuy = true;
                    quantity = (int)(capital / currentPrice);
                    if (quantity > 0)
                    {
                        capital -= quantity * currentPrice;
                        stockOrder.Add(new Order(quantity, currentPrice, currentDate, OrderType.BUY));
                    }
                    else
                    {
                        Console.WriteLine("Your capital is not enough to buy one stock.");
                    }
                   
                }
                else if (flagBuy == true && quantity > 0)
                {
                    quantity = stockOrder.LastOrDefault().Quantity;
                    double buyPrice = stockOrder.Last().TransactionPrice;

                    if (currentPrice > buyPrice * 1.05)
                    {
                        capital += quantity * currentPrice;
                        flagBuy = false;
                        stockOrder.Add(new Order(quantity, currentPrice, currentDate, OrderType.SELL));

                        //Console.WriteLine("Order Date " + currentDate.ToShortDateString() +
                        //    " Capital: " + capital +
                        //    " Price: " + currentPrice +
                        //    " Quantity: " + quantity +
                        //    " Type: " + "SELL");
                    }
                }
            }
        }
    }
}
