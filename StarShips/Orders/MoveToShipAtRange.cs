using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using StarShips.Orders.Delegates;
using System.Drawing;
using StarShips.Locations;
using StarShips.Orders.Interfaces;

namespace StarShips.Orders
{
    [Serializable]
    public class MoveToShipAtRange : ShipOrder, ISerializable, IMoveOrder
    {
        public event OrderDelegates.ShipMoveEvent OnShipMove;
        public override string ExecuteOrder(Ship ship)
        {
            string result = "Could Not Move";
            if (OnShipMove != null)
            {
                //Point targetCurrentLoc = ((Ship)this.OrderValues[0]).Position;
                Point sourceLoc = ship.Position;
                Point targetLoc = ((Ship)this.OrderValues[0]).Position;
                LocationCollection locations = (LocationCollection)this.OrderValues[2];
                int range = (int)this.OrderValues[1];
                Point targetCurrentLoc = locations.GetTargetPointOnRadius(sourceLoc, targetLoc, range, locations.GetLength(0), locations.GetLength(1));
                while (ship.Position != targetCurrentLoc && ship.MP.Current > 0)
                {
                    Point from = ship.Position;
                    Point to = locations.MoveShipToPoint(ship, targetCurrentLoc);
                    OnShipMove(this, new EventArgs(), ship.Image,from, to, ship.WeaponsFiredAlready);
                }

                result = string.Format("Moved towards {0} at range {1}", ((Ship)OrderValues[0]).ClassName,range);
            }
            this.IsCompleted = true;
            return result;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TargetShip", (Ship)OrderValues[0]);
            info.AddValue("TargetRange", (int)OrderValues[1]);
            info.AddValue("Locations", (LocationCollection)OrderValues[2]);
        }

        public override string ToString()
        {
            return string.Format("Move to {0} at range {1}", ((Ship)OrderValues[0]).ClassName, (int)OrderValues[1]);
        }

        #region Constructors
        public MoveToShipAtRange(Ship targetShip, int range, LocationCollection locations)
        {
            this.OrderValues = new object[3];
            this.OrderValues[0] = targetShip;
            this.OrderValues[1] = range;
            this.OrderValues[2] = locations;
        }
        public MoveToShipAtRange(object[] OrderValues)
        {
            this.OrderValues = OrderValues;
        }
        public MoveToShipAtRange(SerializationInfo info, StreamingContext ctxt)
        {
            this.OrderValues = new object[3];
            this.OrderValues[0] = (Ship)info.GetValue("TargetShip", typeof(Ship));
            this.OrderValues[1] = (int)info.GetValue("TargetRange", typeof(int));
            this.OrderValues[2] = (LocationCollection)info.GetValue("Locations", typeof(LocationCollection));
        }

        #endregion
    }
}
