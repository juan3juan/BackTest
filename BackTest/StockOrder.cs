using System;

namespace BackTest
{
    /// <summary>
    /// Order Type
    /// </summary>
    public enum OrderType
    {
        BUY,
        SELL
    }
    /// <summary>
    /// Class of Market Order
    /// </summary>
    public class Order
    {
        #region Property
        /// <summary>
        /// Order Quantity
        /// </summary>
        public int Quantity;
        /// <summary>
        /// Order Transaction Price
        /// </summary>
        public double TransactionPrice;
        /// <summary>
        /// Order Book Date
        /// </summary>
        public DateTime TransactionDate;
        /// <summary>
        /// Order Type. Sell or Buy
        /// </summary>
        public OrderType Type;
        #endregion Property

        public Order(int quantity, double price, DateTime date, OrderType type)
        {
            Quantity = quantity;
            TransactionPrice = price;
            TransactionDate = date;
            Type = type;
        }
    }
}
