using System;
using System.Collections.Generic;
using System.Text;

namespace BackTest
{
    class BackTestBiz
    {

        public static void Run(Dictionary<string, List<PriceStore>> timeseries)
        {
            List<PriceStore> ps = timeseries["BABA"];
            List<StockOrder> stockOrder = new List<StockOrder>();
            bool flagBuy = false;
            bool flagSell = true;
            double capital = 10000;
            double quantity = 0;

            for (int i=0; i<ps.Count-1; i++)
            {
                if ((ps[i].ClosePrice > (ps[i+1].ClosePrice)*1.03) && flagSell==true)
                {
                    flagBuy = true;
                    quantity = capital / ps[i+1].ClosePrice;
                    flagSell = false;
                    stockOrder.Add(new StockOrder(capital, quantity, ps[i + 1].ClosePrice, true));

                    Console.WriteLine("Order in " + (i + 1) + " day: " +
                        "Capital: " + capital +
                        "Price: " + ps[i + 1].ClosePrice +
                        "Quantity: " + quantity +
                        "Status: " + "Buy");
                }
                if ((ps[i].ClosePrice < (ps[i + 1].ClosePrice) * 1.05) && flagBuy == true)
                {
                    flagSell = true;
                    capital = quantity * ps[i + 1].ClosePrice;
                    flagBuy = false;
                    stockOrder.Add(new StockOrder(capital, quantity, ps[i + 1].ClosePrice, false));

                    Console.WriteLine("Order in " + (i + 1) + " day: " +
                       "Capital: " + capital +
                        "Price: " + ps[i + 1].ClosePrice +
                        "Quantity: " + quantity +
                        "Status: " + "sell");
                }
                else
                    continue;
            }
        }
    }
}
