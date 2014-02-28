using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarShips.Orders.Delegates
{
    public class OrderEventArgs : EventArgs
    {
        object[] _orderValues = new object[1];
        public object[] OrderValues { get { return _orderValues; } set { _orderValues = value; } }

        public OrderEventArgs(object[] orderValues)
        {
            this._orderValues = orderValues;
        }
    }
}
