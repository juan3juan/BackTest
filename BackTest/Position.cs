using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackTest.SecurityLib;

namespace BackTest
{
    /// <summary>
    /// Class of position info (price can be obtained through CurrentSecurity)
    /// </summary>
    public class Position
    {
       // public double CurrentPrice; // price is not supposed to be here
        public Security CurrentSecurity;
        public int Quantity;
    }
}
