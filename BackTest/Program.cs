using System;
using System.Collections.Generic;
namespace BackTest
{
    class Program
    {
        static Dictionary<string, List<PricingData>> timeSeries=new Dictionary<string, List<PricingData>>();

        static void Main(string[] args)
        {
            if (timeSeries.Count==0)
            {
                DataAccess dataAccess = new DataAccess();
                timeSeries = dataAccess.ReadDataFile();
            }

            double capital = 0;
            Console.WriteLine("Please input your capital: ");

            if (double.TryParse(Console.ReadLine(), out capital))
            {
                BackTestBiz.Run(timeSeries, capital);
            }
            else
            {
                Console.WriteLine("The input is not valid.");
            }
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    } 
}
