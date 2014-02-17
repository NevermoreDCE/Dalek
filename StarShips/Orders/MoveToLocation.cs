using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using StarShips.Orders.Delegates;
using StarShips.Orders.Interfaces;
using StarShips.Locations;

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
                Point sourceLoc = ship.Position;
                Point targetLoc = (Point)this.OrderValues[0];
                LocationCollection locations = (LocationCollection)this.OrderValues[1];
                while (ship.Position != targetLoc && ship.MP.Current > 0)
                {
                    Point from = ship.Position;
                    Point to = locations.MoveShipToPoint(ship, targetLoc);
                    OnShipMove(this, new EventArgs(), ship.Image, from, to, ship.WeaponsFiredAlready);
                }
                result = string.Format("Moved towards {0},{1}", ((Point)OrderValues[0]).X, ((Point)OrderValues[0]).Y);
            }
            this.IsCompleted = true;
            // check if action should be removed on completion
            if (ship.Position == (Point)OrderValues[0])
                ship.CompletedOrders.Add(this);
            return result;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TargetLocation", (Point)OrderValues[0]);
            info.AddValue("LocationCollection", (LocationCollection)OrderValues[1]);
        }

        public override string ToString()
        {
            return string.Format("Move to {0},{1}", ((Point)OrderValues[0]).X, ((Point)OrderValues[0]).Y);
        }

        #region Constructors
        public MoveToLocation(Point targetLocation, LocationCollection locations)
        {
            this.OrderValues = new object[2];
            this.OrderValues[0] = targetLocation;
            this.OrderValues[1] = locations;
        }
        public MoveToLocation(object[] OrderValues)
        {
            this.OrderValues = OrderValues;
        }
        public MoveToLocation(SerializationInfo info, StreamingContext ctxt)
        {
            this.OrderValues = new object[2];
            this.OrderValues[0] = (Point)info.GetValue("TargetLocation", typeof(Point));
            this.OrderValues[1] = (LocationCollection)info.GetValue("LocationCollection", typeof(LocationCollection));
        }

        #endregion
    }
}
