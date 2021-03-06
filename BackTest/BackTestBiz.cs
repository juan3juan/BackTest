﻿using System;
using System.Linq;
using System.Collections.Generic;
using AllocationEngine;
using DataContract;
using DataAccessLib;

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
        private static List<Position> GetCurrentPositions(/*double currentPrice*/)
        {
            List<Position> Positions = new List<Position>();
            foreach (var group in orders.GroupBy(o=>o.CurrentSecurity.SecurityID))
            {
                //first version-quantity has signal in Order class
                //second version-realize in Strategy, Buy+, Sell-
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
            string key = securityMaster.First().Key;
            Security security = securityMaster[key];
            List<PricingData> ps = security.SecurityPricingData;
            double Cash = capital;
            
            // loop through the data read in
            for (int i=1; i<ps.Count; i++)
            {
                // params for Strategy
                double currentPrice = ps[i].ClosePrice;
                DateTime currentDate = ps[i].Date;

                #region store info to AccountLevel
                // used to transfer data to Account Class and then show these infos to customer
                AccountLevelInfo currentAccountInfo = new AccountLevelInfo();
                currentAccountInfo.Date = currentDate;
                currentAccountInfo.CurrentCash = Cash;
                currentAccountInfo.CurrentPositions = GetCurrentPositions(/*currentPrice*/);
                accountInfos.Add(currentAccountInfo);
                #endregion store info to AccoutLevel

                #region Build DataContract
                StrategyDataContract dataContract = new StrategyDataContract();
                dataContract.CurrentCash = Cash;
                SecPosition secPosition = new SecPosition();
                secPosition.SecurityID = key;
                secPosition.currentDate = currentDate;
                secPosition.CurrentPrice = ps.Where(p => DateTime.Compare(p.Date, currentDate) <= 0)
                             .Select(p => p.ClosePrice)
                             .ToList();

                if (currentAccountInfo.CurrentPositions.Count > 0)
                {
                    secPosition.BuyPrice = orders.Last().TransactionPrice;
                    secPosition.PositionQuantity = currentAccountInfo.CurrentPositions.First().Quantity;
                }

                //Fill the DataContract that will be sent to Allocation Engine
                dataContract.SecPositions.Add(secPosition);
                #endregion Build DataContract

                #region Run Strategy
                Dictionary<string, int> result = Strategy.ExecuteStrategy(dataContract);
                #endregion Run Strategy

                #region Create Order by the Quantity returned by the AllocationEngine
                int quantity = result.Count > 0 ? result.First().Value : 0;
                if (quantity > 0)  
                {
                    Cash -= quantity * currentPrice;
                    orders.Add(new Order(security, quantity, currentPrice, currentDate, OrderType.BUY));
                    Console.WriteLine("Order Date " + currentDate.ToShortDateString() +
                                        " Price: " + currentPrice +
                                        " Quantity: " + quantity +
                                        " Type: " + "BUY");
                }
                else if (quantity<0)
                {
                    Cash -= quantity * currentPrice;
                    orders.Add(new Order(security, quantity, currentPrice, currentDate, OrderType.SELL));
                    Console.WriteLine("Order Date " + currentDate.ToShortDateString() +
                                        " Price: " + currentPrice +
                                        " Quantity: " + quantity +
                                        " Type: " + "SELL");
                }
                #endregion Create Order by the Quantity returned by the AllocationEngine
            }
            return orders;
        }

        public static List<Order> RunMulti(IStrategy Strategy, Dictionary<string, Security> securityMaster, double capital)
        {
            List<DateTime> Dates = securityMaster
                                    .Values.First()
                                    .SecurityPricingData.Select(p => p.Date)
                                    .ToList();
            double Cash = capital;
            foreach (DateTime date in Dates)
            {
                if (DateTime.Compare(date, Dates.First()) == 0)
                    continue;
                #region store info to AccountLevel
                // used to transfer data to Account Class and then show these infos to customer
                AccountLevelInfo currentAccountInfo = new AccountLevelInfo();
                currentAccountInfo.Date = date;
                currentAccountInfo.CurrentCash = Cash;
                currentAccountInfo.CurrentPositions = GetCurrentPositions();
                accountInfos.Add(currentAccountInfo);
                #endregion store info to AccountLevel

                #region Build DataContract
                StrategyDataContract dataContract = new StrategyDataContract();
                dataContract.CurrentCash = Cash;

                foreach (var sec in securityMaster)
                {
                    SecPosition secPosition = new SecPosition();
                    secPosition.SecurityID = sec.Key;
                    secPosition.currentDate = date;
                    secPosition.CurrentPrice = sec.Value.SecurityPricingData.Where(p => DateTime.Compare(p.Date, date) <= 0)
                                 .Select(p => p.ClosePrice)
                                 .ToList();

                    if(currentAccountInfo.CurrentPositions.Where(p => p.CurrentSecurity.SecurityID == sec.Key).Count() > 0)
                    {
                        if (orders.Where(p => p.CurrentSecurity.SecurityID == sec.Key).Count() > 0)
                        {
                                secPosition.BuyPrice = orders.Where(p => p.CurrentSecurity.SecurityID == sec.Key)
                                     .Last().TransactionPrice;
                                secPosition.PositionQuantity = currentAccountInfo.CurrentPositions.Where(p => p.CurrentSecurity.SecurityID == sec.Key)
                                         .Last().Quantity;
                            
                        }
                    }

                    //Fill the DataContract that will be sent to Allocation Engine
                    dataContract.SecPositions.Add(secPosition);
                }
                #endregion Build DataContract

                #region Run Strategy
                Dictionary<string, int> result = Strategy.ExecuteStrategy(dataContract);
                #endregion Run Strategy

                #region Create Order by the Quantity returned by the AllocationEngine
                int quantity;
                foreach (var group in result.GroupBy(o => o.Key))
                {
                    double currentPrice = securityMaster[group.Key]
                              .SecurityPricingData
                              .Where(p => p.Date == date)
                              .Select(p => p.ClosePrice).First();
                    quantity = group.Count() > 0 ? group.First().Value : 0;
                    if (quantity > 0)
                    {
                        Cash -= quantity * currentPrice;
                        orders.Add(new Order(securityMaster[group.Key], quantity, currentPrice, date, OrderType.BUY));
                        Console.WriteLine("Order Date " + date.ToShortDateString() +
                                           " SecurityID: " + group.Key +
                                           " Price: " + currentPrice +
                                           " Quantity: " + quantity +
                                           " Type: " + "BUY");
                    }
                    else if (quantity < 0)
                    {
                        Cash -= quantity * currentPrice;
                        orders.Add(new Order(securityMaster[group.Key], quantity, currentPrice, date, OrderType.SELL));
                        Console.WriteLine("Order Date " + date.ToShortDateString() +
                                            " SecurityID: " + group.Key +
                                            " Price: " + currentPrice +
                                            " Quantity: " + quantity +
                                            " Type: " + "SELL");
                    }
                }
                #endregion Create Order by the Quantity returned by the AllocationEngine

            }
            return orders;
        }


    }
}
