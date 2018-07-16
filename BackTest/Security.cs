using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTest
{
    public class Security
    {
        public string SecurityID;

        public double GetPrice(DateTime date)
        {
            List<PricingData> pricingDatas = new List<PricingData>();
            pricingDatas = DataAccess.TimeSeries[SecurityID];
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
            return pricingDatas.Where(p => DateTime.Compare(date, p.Date) == 0)
                .First()
                .ClosePrice;

        }
    }
}
