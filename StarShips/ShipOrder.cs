using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace StarShips
{
    public abstract class ShipOrder
    {
        #region Parameters
        object[] _orderValues = new object[1];
        public object[] OrderValues { get { return _orderValues; } set { _orderValues = value; } }
        #endregion


        public abstract string ExecuteOrder(Ship ship);
        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
        public abstract override string ToString();
    }
}
