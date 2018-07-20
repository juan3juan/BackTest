using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib
{
    public class Security
    {
        /// <summary>
        /// Class used to store security ID and getPrice
        /// </summary>
        /// <param name="securityID"></param>
        public Security(string securityID)
        {
            SecurityID = securityID;
        }

        public string SecurityID;


        public List<PricingData> SecurityPricingData;

        /// <summary>
        /// return the price of certain position in certain date
        /// </summary>
        /// <param name="date">give the date want to calculate price</param>
        /// <returns></returns>
        public double GetPrice(DateTime date)
        {
            //double price = 0;
            //for(int i=0; i<pricingDatas.Count; i++)
            //{
            //    int result = DateTime.Compare(date, pricingDatas[i].Date);
            //    if(result == 0)
            //    {
            //        price = pricingDatas[i].ClosePrice;
            //    }
            //}

            //foreach(PricingData pd in pricingDatas)
            //{
            //    int result = DateTime.Compare(date, pd.Date);
            //    if (result == 0)
            //    {
            //        price = pd.ClosePrice;
            //    }
            //}

            //var price = from pd in pricingDatas
            //        where (DateTime.Compare(date, pd.Date) == 0)
            //        select pd.ClosePrice;


            //return price.First();
            return SecurityPricingData.Where(p => DateTime.Compare(date, p.Date) == 0)
                .First()
                .ClosePrice;

        }
    }
}
