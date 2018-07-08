using System;

namespace BackTest
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello World!");
            DataAccess dataAccess = new DataAccess();
            dataAccess.ReadDataFile();

            BackTestBiz.Run(dataAccess.timeSeries);

            Console.ReadLine();
        }
    } 
}
