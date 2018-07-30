using AllocationEngine;
using BackTest;
using DataAccessLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestNet
{
    class Program
    {
        static List<Order> orders = new List<Order>();
        static List<AccountLevelInfo> accountInfos = new List<AccountLevelInfo>();
        static Dictionary<string, Security> SecurityMaster = new Dictionary<string, Security>();

        static void Main(string[] args)
        {

            double capital = 10000;
            SecurityMaster = DataAccess.SecurityMaster;
            IStrategy Strategy = new StrategyService();
            BackTestBiz.Run(Strategy, SecurityMaster, capital);
            accountInfos = BackTestBiz.accountInfos;
            //Console.WriteLine("Please input your capital: ");

            //if (double.TryParse(Console.ReadLine(), out capital))
            //{
            //    BackTestBiz.Run(timeSeries, capital);
            //    accountInfos = BackTestBiz.accountInfos;
            //}
            //else
            //{
            //    Console.WriteLine("The input is not valid.");
            //}
            OutPutAccountInfo();

            Console.ReadLine();
        }

        private static void OutPutAccountInfo()
        {
            foreach (AccountLevelInfo info in accountInfos)
            {
                Console.WriteLine("----------------------------------");
                Console.WriteLine("Date: {0},  Cash: {1},  TotalCapital: {2}", info.Date, info.CurrentCash, info.CurrentCapital);
            }
        }
    }
}
