using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace BackTest
{
    public class DataAccess
    {
        public Dictionary<string, List<PriceStore>> timeSeries = new Dictionary<string, List<PriceStore>>();
        public void ReadDataExcel()
        {
            //Read File
            //Excel.Application excelApp = new Excel.Application();
            //if(excelApp != null)
            //{
            //    Excel.Workbook excelWorkbook = excelApp.Workbooks.Open(@"D:\Yury\Program\BackTest\DataFile\BABA.csv");
            //    Excel.Worksheet excelWorksheet = (Excel.Worksheet)excelWorkbook.Sheets["Close"];

            //    Excel.Range excelRange = excelWorksheet.UsedRange;
            //    int rowCount = excelRange.Rows.Count;
            //    int colCount = excelRange.Columns.Count;

            //    for (int i = 1; i <= rowCount; i++)
            //    {
            //        Excel.Range range = (excelWorksheet.Cells[i, "Close"] as Excel.Range);
            //        string cellValue = range.Value.ToString();

            //        Console.WriteLine("test" + "i", cellValue);

            //    }

            //}

            //List<PriceStore> ps = new List<PriceStore>();

            //TimeSeries.Add("BABA", ps);
     
        }

        public void ReadDataFile()
        {
            
            //char[] delimiter = new char[] { '\t' };
            //string columnHeaders = 
            string[] lines = System.IO.File.ReadAllLines(@"D:\Yury\Program\BackTest\DataFile\BABA.txt");

            List<PriceStore> ps = new List<PriceStore>();
            System.Console.WriteLine("Contents of text: ");
            foreach (string line in lines)
            {
                string[] values = line.Split("\t");

                DateTime date;
                DateTime.TryParse(values[0].Trim(), out date);
                double priceStore;
                double.TryParse(values[1].Trim(), out priceStore);
                ps.Add(new PriceStore(date, priceStore));
                Console.WriteLine(ps.Last().Date.ToString()+" "+ps.Last().ClosePrice.ToString());
              
            }
            timeSeries.Add("BABA", ps);
        }
    }
}
