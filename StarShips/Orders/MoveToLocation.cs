using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using StarShips.Orders.Delegates;
using StarShips.Orders.Interfaces;

namespace StarShips.Orders
{
    [Serializable]
    public class MoveToLocation : ShipOrder, ISerializable, IMoveOrder
    {
        public event OrderDelegates.ShipMoveEvent OnShipMove;
        public override string ExecuteOrder(Ship ship)
        {
            string result = "Could Not Move";
            if (OnShipMove != null)
            {
                OrderEventArgs e = new OrderEventArgs(this.OrderValues);
                OnShipMove(this,e, ship, true);
                result = string.Format("Moved towards {0},{1}", ((Point)OrderValues[0]).X, ((Point)OrderValues[0]).Y);
            }
            return result;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TargetLocation", (Point)OrderValues[0]);
        }

        public override string ToString()
        {
            return string.Format("Move to {0},{1}", ((Point)OrderValues[0]).X, ((Point)OrderValues[0]).Y);
        }

        #region Constructors
        public MoveToLocation(Point targetLocation)
        {
            this.OrderValues[0] = targetLocation;
        }
        public MoveToLocation(object[] OrderValues)
        {
            this.OrderValues = OrderValues;
        }
        public MoveToLocation(SerializationInfo info, StreamingContext ctxt)
        {
            this.OrderValues[0] = (Point)info.GetValue("TargetLocation", typeof(Point));
        }

        #endregion
    }
}
