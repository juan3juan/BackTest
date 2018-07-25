using System;
using System.Linq;
using System.Collections.Generic;
using AllocationEngine;
using DataContract;
using DataAccessLib;
using Newtonsoft.Json;
using RestSharp;

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

                #region store info to AccoutLevel
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
                //Dictionary<string, int> result = Strategy.ExecuteStrategy(dataContract);
                string serviceresult=strategyService(dataContract);
                Dictionary<string, int> result = null;//Strategy.ExecuteStrategy(dataContract);
                if (serviceresult!=string.Empty)
                {
                    try
                    {
                        result = JsonConvert.DeserializeObject<Dictionary<string, int>>(serviceresult);
                    }
                    catch(Exception ex)
                    {
                        result = new Dictionary<string, int>();
                    }
                }
                #endregion Run Strategy

                #region Create Order by the Quantity returned by the AllocationEngine
                int quantity = result.Count > 0 ? result.First().Value : 0;
                if (quantity > 0)
                {
                    Cash -= quantity * currentPrice;
                    orders.Add(new Order(security, quantity, currentPrice, currentDate, OrderType.BUY));
                    Console.WriteLine("Order Date " + currentDate.ToShortDateString() +
                    " Capital: " + capital +
                    " Price: " + currentPrice +
                    " Quantity: " + quantity +
                    " Type: " + "BUY");
                }
                else if (quantity<0)
                {
                    Cash -= quantity * currentPrice;
                    orders.Add(new Order(security, quantity, currentPrice, currentDate, OrderType.SELL));
                    Console.WriteLine("Order Date " + currentDate.ToShortDateString() +
                                        " Capital: " + capital +
                                        " Price: " + currentPrice +
                                        " Quantity: " + quantity +
                                        " Type: " + "SELL");
                }
                #endregion Create Order by the Quantity returned by the AllocationEngine
            }
            return orders;
        }

        private static string strategyService(StrategyDataContract dataContract)
        {
            var client = new RestClient("https://localhost:44332/api/AllocationEngine");
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest("Strategy", Method.POST);

            //request.AddJsonBody(JsonConvert.SerializeObject(dataContract));
            request.AddJsonBody(dataContract);

            IRestResponse response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Content; // raw content as string
            }
            return string.Empty;
        }
    }
}
