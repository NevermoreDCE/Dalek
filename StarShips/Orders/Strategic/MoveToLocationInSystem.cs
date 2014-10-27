using StarShips.Locations;
using StarShips.Orders.Delegates;
using StarShips.Orders.Interfaces;
using StarShips.StarSystems;
using StarShips.Stellars;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace StarShips.Orders.Strategic
{
    [Serializable]
    public class MoveToLocationInSystem : ShipOrder, ISerializable, IStrategicMoveOrder
    {
        public event OrderDelegates.StrategicShipMoveEvent OnShipMove;
        
        public override List<string> ExecuteOrder(Ship ship)
        {
            List<string> result = new List<string>();
            result.Add(moveShip(ship));
            this.IsCompleted = true;
            // check if action should be removed on completion
            if (ship.StrategicPosition == (Point)OrderValues[0] && ship.StrategicSystem == (StarSystem)OrderValues[1])
                ship.CompletedOrders.Add(this);
            return result;
        }

        private string moveShip(Ship ship)
        {
            string result = "";
            Point targetLoc = (Point)this.OrderValues[0];
            StarSystem targetSystem = (StarSystem)this.OrderValues[1];

            // loop: move towards target until MP=0 or loc = target loc
            while (ship.MP.Current > 0 && ship.StrategicPosition != targetLoc)
            {
                // check if in target system
                if (ship.StrategicSystem == targetSystem)
                {                      
                    // move ship towards TargetLoc
                    Point from = ship.StrategicPosition;
                    Point to = ship.StrategicSystem.StrategicLocations.MoveShipToPoint(ship, ship.StrategicPosition, targetLoc);
                    ship.StrategicPosition = to;
                    if (OnShipMove != null)
                        OnShipMove(this, new EventArgs(), ship.Image, ship.StrategicSystem, from, to);
                    result = string.Format("Moved towards {0},{1} in {2}", targetLoc.X, targetLoc.Y, ship.StrategicSystem.Name);
                }
                // else not in target system
                else
                {
                    // find next system
                    StarSystem nextSystem = ship.StrategicSystem.StrategicLocations.GetNextSystem(ship.StrategicSystem, targetSystem);
                    // find loc of Warp Point to next system
                    Point WPLoc = ship.StrategicSystem.GetWarpPointPosition(nextSystem);
                    // move ship towards WPLoc
                    Point from = ship.StrategicPosition;
                    Point to = ship.StrategicSystem.StrategicLocations.MoveShipToPoint(ship,ship.StrategicPosition, WPLoc);
                    ship.StrategicPosition = to;
                    if (OnShipMove != null)
                        OnShipMove(this, new EventArgs(), ship.Image, ship.StrategicSystem, from, to);
                    result = String.Concat(result, string.Format("Moved towards {0},{1} in {2} ", targetLoc.X, targetLoc.Y, ship.StrategicSystem.Name));
                    // if at warp point, warp through
                    if(ship.StrategicPosition == WPLoc)
                    {
                        Point warpFrom = ship.StrategicPosition;
                        Point warpTo = ship.StrategicSystem.StrategicLocations.WarpShipToSystem(ship, nextSystem);
                        if (OnShipMove != null)
                            OnShipMove(this, new EventArgs(), ship.Image, ship.StrategicSystem, warpFrom, warpTo);
                        result = String.Concat(result, string.Format("Warped to {0},{1} in {2} ", warpTo.X, warpTo.Y, nextSystem.Name));
                    }
                }
            }
            return result;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TargetLocation", (Point)this.OrderValues[0]);
            info.AddValue("TargetSystem", (StarSystem)this.OrderValues[1]);
        }

        public override string ToString()
        {
            return string.Format("Move to {0},{1} in {2}", ((Point)OrderValues[0]).X, ((Point)OrderValues[0]).Y, ((StarSystem)OrderValues[1]).Name);
        }

        #region Constructors
        public MoveToLocationInSystem(Point targetLocation, StarSystem targetSystem)
        {
            this.OrderValues = new object[2];
            this.OrderValues[0] = targetLocation;
            this.OrderValues[1] = targetSystem;
        }
        public MoveToLocationInSystem(object[] OrderValues)
        {
            this.OrderValues = OrderValues;
        }
        public MoveToLocationInSystem(SerializationInfo info, StreamingContext ctxt)
        {
            this.OrderValues = new object[2];
            this.OrderValues[0] = (Point)info.GetValue("TargetLocation", typeof(Point));
            this.OrderValues[1] = (StarSystem)info.GetValue("TargetSystem", typeof(StarSystem));
        }
        #endregion
    }
}
