using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTest
{
    public class PriceStore
    {
        public DateTime Date;
        public double ClosePrice;

        public PriceStore(DateTime date, double closePrice)
        {
            Date = date;
            ClosePrice = closePrice;
        }

    }
}
