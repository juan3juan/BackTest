using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTest
{
    public class StockOrder
    {
        public double Capital;
        public double Quantity;
        public double Price;
        public bool Status;



        public StockOrder(double capital, double quantity, double price, bool status)
        {
            Capital = capital;
            Quantity = quantity;
            Price = price;
            Status = status;
        }

        public static implicit operator List<object>(StockOrder v)
        {
            throw new NotImplementedException();
        }
    }
}
