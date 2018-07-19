using System;

namespace BackTest.SecurityLib
{
    /// <summary>
    /// Class used to store Security Prcing
    /// </summary>
    public class PricingData
    {
        #region Property
        /// <summary>
        /// Date of the Prcing
        /// </summary>
        public DateTime Date;
        /// <summary>
        /// Market Close Price
        /// </summary>
        public double ClosePrice;
        #endregion Property

        public PricingData(DateTime date, double closePrice)
        {
            Date = date;
            ClosePrice = closePrice;
        }

    }
}
