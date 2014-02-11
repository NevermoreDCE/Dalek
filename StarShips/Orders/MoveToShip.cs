using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using StarShips.Orders.Delegates;
using System.Drawing;
using StarShips.Orders.Interfaces;

namespace StarShips.Orders
{
    [Serializable]
    public class MoveToShip : ShipOrder, ISerializable, IMoveOrder
    {
        public event OrderDelegates.ShipMoveEvent OnShipMove;
        public override string ExecuteOrder(Ship ship)
        {
            string result = "Could Not Move";
            if (OnShipMove != null)
            {
                Point targetCurrentLoc = ((Ship)this.OrderValues[0]).Position;
                object[] orderVals = new object[1];
                orderVals[0] = targetCurrentLoc;
                OrderEventArgs e = new OrderEventArgs(orderVals);
                OnShipMove(this, e, ship, true);
                result = string.Format("Moved towards {0}", ((Ship)OrderValues[0]).Name);
            }
            return result;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TargetShip", (Ship)OrderValues[0]);
        }

        public override string ToString()
        {
            return string.Format("Move to {0}", ((Ship)OrderValues[0]).Name);
        }

        #region Constructors
        public MoveToShip(Ship targetShip)
        {
            this.OrderValues[0] = targetShip;
        }
        public MoveToShip(object[] OrderValues)
        {
            this.OrderValues = OrderValues;
        }
        public MoveToShip(SerializationInfo info, StreamingContext ctxt)
        {
            this.OrderValues[0] = (Ship)info.GetValue("TargetShip", typeof(Ship));
        }

        #endregion
    }
}
