using System;
using System.Linq;
using System.Collections.Generic;
using BackTest.SecurityLib;
using AllocationEngine;

namespace BackTest
{
    class BackTestBiz
    {
        /// <summary>
        /// Realize the business logic of order
        /// </summary>
        public static List<Order> orders = new List<Order>();
        public static List<AccountLevelInfo> accountInfos = new List<AccountLevelInfo>();

        /// <summary>
        /// calcate the position of certain stock in certain day with its price at that day
        /// </summary>
        /// <param name="currentPrice">refers to certain stock price in certain day</param>
        /// <returns></returns>
        private static List<Position> GetCurrentPositions(double currentPrice)
        {
            List<Position> Positions = new List<Position>();
            foreach (var group in orders.GroupBy(o=>o.CurrentSecurity.SecurityID))
            {
                //quantity has signal in Order class
                int quantity = group.Sum(o => o.Quantity); 
                
                if (quantity != 0)
                {
                    Position Position = new Position();
                    Position.CurrentSecurity = group.First().CurrentSecurity; 
                    Position.Quantity = quantity;
                    Positions.Add(Position);
                }
            }

            return Positions;

        }

        /// <summary>
        /// obtain the raw data from SecurityMaster and initial cash from capital
        /// </summary>
        /// <param name="securityMaster">transfer security with name</param>
        /// <param name="capital">initial capital, here means the initial cash</param>
        /// <returns></returns>
        public static List<Order> Run(IStrategy Strategy,Dictionary<string, Security> securityMaster, double capital)
        {
            string key = "BABA";
            Security security = securityMaster[key];
            List<PricingData> ps = security.SecurityPricingData;
            double Cash = capital;
            
            bool flagBuy = false;
            // traversal the data read in 
            for (int i=1; i<ps.Count; i++)
            {
                double CurrentCash=Cash;
                Dictionary<string, int> CurrentPostions=new Dictionary<string, int>();
                Dictionary<string, List<double>> CurrentPrice=new Dictionary<string, List<double>>();
                Dictionary<string, double> BuyPrice = new Dictionary<string, double>();

                double previousPrice = ps[i-1].ClosePrice;
                double currentPrice = ps[i].ClosePrice;
                DateTime currentDate = ps[i].Date;

                #region store info to AccoutLevel
                // used to transfer data to Account Class and then show these infos to customer
                AccountLevelInfo currentAccountInfo = new AccountLevelInfo();
                currentAccountInfo.Date = currentDate;
                currentAccountInfo.CurrentCash = Cash;
                currentAccountInfo.CurrentPositions = GetCurrentPositions(currentPrice);             
                accountInfos.Add(currentAccountInfo);
                #endregion store info to AccoutLevel

                currentAccountInfo.CurrentPositions.ForEach(p => CurrentPostions.Add(p.CurrentSecurity.SecurityID, p.Quantity));
                CurrentPrice.Add(key, ps.Where(p=>DateTime.Compare(p.Date,currentDate)<=0).Select(p=>p.ClosePrice).ToList());
                if (CurrentPostions.Count>0)
                {
                    BuyPrice.Add(key, orders.Last().TransactionPrice);
                }

                Dictionary<string, int> result= Strategy.ExecuteStrategy(CurrentCash, CurrentPostions, BuyPrice, CurrentPrice);

                int quan = result.First().Value;
                if (quan > 0)
                {
                    Cash -= quan * currentPrice;
                    orders.Add(new Order(security, quan, currentPrice, currentDate, OrderType.BUY));
                    Console.WriteLine("Order Date " + currentDate.ToShortDateString() +
                    " Capital: " + capital +
                    " Price: " + currentPrice +
                    " Quantity: " + quan +
                    " Type: " + "BUY");
                }
                else if (quan<0)
                {
                    Cash -= quan * currentPrice;
                    orders.Add(new Order(security, quan, currentPrice, currentDate, OrderType.SELL));
                    Console.WriteLine("Order Date " + currentDate.ToShortDateString() +
                                        " Capital: " + capital +
                                        " Price: " + currentPrice +
                                        " Quantity: " + quan +
                                        " Type: " + "SELL");
                }
            }

            return orders;
        }
    }
}
