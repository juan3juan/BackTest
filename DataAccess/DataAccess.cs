﻿using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DataAccessLib
{
    /// <summary>
    /// This whole class is Static
    /// </summary>
    public class DataAccess
    {
        private DataAccess()
        {

        }

        private static Dictionary<string, Security> securityMaster = null;
        public static Dictionary<string, Security> SecurityMaster
        {
            get
            {
                if (securityMaster == null)
                {
                    // if not new, then the method will become a dead loop with SecurityMaster add
                    securityMaster = new Dictionary<string, Security>();
                    DataFromService();
                }
                return securityMaster;
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
        /// <summary>
        /// Read data from file
        /// </summary>
        private static void ReadDataFile()
        {
            string securityKey = "BABA";


            Security security = new Security(securityKey);
            var x = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            #region Load Security Pricing Data
            string path = @"..\..\..\DataAccess\DataFile\BABA.txt";

            List<PricingData> ps = new List<PricingData>();

            Console.WriteLine("Contents of text: ");

            File.ReadAllLines(path).ToList().ForEach(line =>
            {
                string[] values = line.Split('\t');
                DateTime date;
                DateTime.TryParse(values[0].Trim(), out date);
                double priceStore;
                double.TryParse(values[1].Trim(), out priceStore);
                ps.Add(new PricingData(date, priceStore));
                Console.WriteLine(ps.Last().Date.ToString() + " " + ps.Last().ClosePrice.ToString());
            });
            #endregion Load Security Pricing Data

            security.SecurityPricingData = ps;

            // securityMaster means add to securityMaster directly
            // SecurityMaster means execute SecurityMaster Get method again, and then add to securityMaster
            securityMaster.Add(securityKey, security);

        }

        private static void DataFromService()
        {
            var client = new RestClient("https://localhost:44350/api/DataAccess");
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest("DataFromFile", Method.GET);

            //request.AddJsonBody(JsonConvert.SerializeObject(dataContract));
            //request.AddJsonBody(dataContract);

            //Dictionary<string, Security> result = null;
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                securityMaster = JsonConvert.DeserializeObject<Dictionary<string, Security>>(response.Content);
            }

        }

        public static void Flush()
        {
            if (SecurityMaster != null)
            {
                //SecurityMaster.Clear();
                securityMaster = null;
            }
        }

    }
}
