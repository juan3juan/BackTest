using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace BackTest
{
    class BackTestBiz
    {
        public static List<Order> orders = new List<Order>();
        public static List<AccountLevelInfo> accountInfos = new List<AccountLevelInfo>();

        private static List<Position> GetCurrentPositions(double currentPrice)
        {
            List<Position> Positions = new List<Position>();
            foreach (var group in orders.GroupBy(o=>o.SecurityID))
            {
                int quantity = group.Sum(o => o.Quantity); //quantity has signal in order

                if (quantity != 0)
                {
                    Position Position = new Position();
                    Position.SecurityID = group.Key; 
                    Position.Quantity = quantity;
                    Position.CurrentPrice = currentPrice;
                    Positions.Add(Position);
                }
            }

            return Positions;

        }

        public static List<Order> Run(Dictionary<string, List<PricingData>> timeseries, double capital)
        {
            string key = "BABA";
            List<PricingData> ps = timeseries[key];
            double Cash = capital;
            
            bool flagBuy = false;
             
            for (int i=0; i<ps.Count-1; i++)
            {
                double previousPrice = ps[i].ClosePrice;
                double currentPrice = ps[i+1].ClosePrice;
                DateTime currentDate = ps[i + 1].Date;

                AccountLevelInfo currentAccountInfo = new AccountLevelInfo();
                currentAccountInfo.Date = currentDate;
                currentAccountInfo.CurrentCash = Cash;
                currentAccountInfo.CurrentPositions = GetCurrentPositions(currentPrice);             
                accountInfos.Add(currentAccountInfo);


                int quantity = 0;

                if ((previousPrice > currentPrice *1.03) && flagBuy == false)
                {
                    flagBuy = true;
                    quantity = (int)(Cash / currentPrice);
                    if (quantity > 0)
                    {
                        Cash -= quantity * currentPrice;
                        orders.Add(new Order(key, quantity, currentPrice, currentDate, OrderType.BUY));
                    }
                    else
                    {
                        Console.WriteLine("Your capital is not enough to buy one stock.");
                    }

                    Console.WriteLine("Order Date " + currentDate.ToShortDateString() +
                                        " Capital: " + capital +
                                        " Price: " + currentPrice +
                                        " Quantity: " + quantity +
                                        " Type: " + "BUY");

                }
                else if (flagBuy == true)
                {
                    quantity = orders.LastOrDefault().Quantity;
                    double buyPrice = orders.Last().TransactionPrice;

                    if (currentPrice > buyPrice * 1.05)
                    {
                        Cash += quantity * currentPrice;
                        flagBuy = false;
                        orders.Add(new Order(key, quantity, currentPrice, currentDate, OrderType.SELL));

                        Console.WriteLine("Order Date " + currentDate.ToShortDateString() +
                            " Capital: " + capital +
                            " Price: " + currentPrice +
                            " Quantity: " + quantity +
                            " Type: " + "SELL");
                    }
                }
            }

            return orders;
        }
    }
}
