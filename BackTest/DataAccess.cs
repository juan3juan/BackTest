using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BackTest
{
    public class DataAccess
    {
        /// <summary>
        /// This whole class is Static
        /// </summary>
        private DataAccess()
        {

        }

        private static Dictionary<string, List<PricingData>> timeSeries = null;
        public static Dictionary<string, List<PricingData>> TimeSeries
        {
            //private set   //private set here means no one can assign to TimeSeries 
            //{
            //    timeSeries = value;
            //}
            get
            {
                if (timeSeries ==null)
                {
                    ReadDataFile();
                }
                return timeSeries;
            }
        }


        #region Read Data from Excel File 
        //public void ReadDataExcel()
        //{
        //    //Read File
        //    Application excelApp = new Application();
        //    if (excelApp != null)
        //    {
        //        Workbook excelWorkbook = excelApp.Workbooks.Open(@"D:\Yury\Program\BackTest\DataFile\BABA.csv");
        //        Worksheet excelWorksheet = (Worksheet)excelWorkbook.Sheets[1];

        //        Range excelRange = excelWorksheet.UsedRange;
        //        int rowCount = excelRange.Rows.Count;
        //        int colCount = excelRange.Columns.Count;

        //        for (int i = 1; i <= rowCount; i++)
        //        {
        //            Range range = (excelWorksheet.Cells[i, "Close"] as Range);
        //            string cellValue = range.Value.ToString();

        //            Console.WriteLine("test" + "i", cellValue);

        //        }

        //    }

        //    List<PriceStore> ps = new List<PriceStore>();

        //   // TimeSeries.Add("BABA", ps);

        //}
        #endregion

        public static void ReadDataFile()
        {
            if (timeSeries == null)
            {


                string path = @"..\..\..\DataFile\BABA.txt";

                List<PricingData> ps = new List<PricingData>();

                Console.WriteLine("Contents of text: ");

                File.ReadAllLines(path).ToList().ForEach(line =>
                            {
                                string[] values = line.Split("\t");
                                DateTime date;
                                DateTime.TryParse(values[0].Trim(), out date);
                                double priceStore;
                                double.TryParse(values[1].Trim(), out priceStore);
                                ps.Add(new PricingData(date, priceStore));
                                Console.WriteLine(ps.Last().Date.ToString() + " " + ps.Last().ClosePrice.ToString());
                            });

                timeSeries.Add("BABA", ps);
            }
        }

        public static void Flush()
        {
            if(timeSeries != null)
            {
                timeSeries.Clear();
                timeSeries = null;
            }
        }

    }
}
