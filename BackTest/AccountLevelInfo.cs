using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTest
{
    /// <summary>
    /// Class used to store Account Info: Date, Cash, Capital
    /// </summary>
    public class AccountLevelInfo
    {
        public DateTime Date;
        public double CurrentCash;
        /// <summary>
        /// CurrentCapital refers to Account capital in total in certain day
        /// </summary>
        public double CurrentCapital
        {
            get
            {
                double capitalFromPositionss = 0; 
                CurrentPositions.ForEach(p =>
                {                                
                    capitalFromPositionss += p.Quantity * p.CurrentSecurity.GetPrice(Date);

                });
                return capitalFromPositionss+ CurrentCash;
            }
        }
        public List<Position> CurrentPositions = new List<Position>();
    }
}
