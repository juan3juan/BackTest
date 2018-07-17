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
        /// Identifier for the Security
        /// </summary>
        public Security CurrentSecurity;
        /// <summary>
        /// Order Quantity
        /// </summary>
        public int Quantity
        {
            set
            {
                quantity = value;
            }
            get
            {
                int direction=Type == OrderType.BUY ? 1 : -1;
                return direction * quantity;
            }
        }
        private int quantity;
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
        public Order(Security security ,int quantity, double price, DateTime date, OrderType type)
        {
            CurrentSecurity = security;
            Quantity = quantity;
            TransactionPrice = price;
            TransactionDate = date;
            Type = type;
        }
    }
}
