using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Orders.Interfaces;
using StarShips.Parts;
using System.Runtime.Serialization;
using StarShips.Orders.Delegates;
using StarShips.Locations;
using System.Diagnostics;

namespace StarShips.Orders
{
    public class FireWeaponAtTarget: ShipOrder,IWeaponOrder
    {
        #region Events
        public event OrderDelegates.WeaponFiredEvent OnWeaponFired;
        #endregion

        public override string ExecuteOrder(Ship ship)
        {
            string result = "Could Not Fire";
            // weapon has subscribed listeners
            if (OnWeaponFired != null)
            {
                Ship target = (Ship)this.OrderValues[1];
                WeaponPart weapon = (WeaponPart)this.OrderValues[0];
                
                // weapon is not destroyed
                if (weapon.IsDestroyed)
                {
                    result = string.Format("{0} is destroyed!", weapon.Name);
                    Debug.WriteLine(string.Format("Resolving {0}, Destroyed", weapon.Name));
                    ship.CountOfResolvingOrders--;
                    ship.CompletedOrders.Add(this);
                }
                else
                {
                    // weapon is reloaded
                    if (!weapon.IsLoaded)
                    {
                        result = string.Format("{0} will be reloaded in {1} turns", weapon.Name, weapon.Reload());
                        Debug.WriteLine(string.Format("Resolving {0}, Reloading", weapon.Name));
                        ship.CountOfResolvingOrders--;
                    }
                    else
                    {

                        // target is valid
                        if (target.HP.Current <= 0)
                        {
                            result = "Target Already Dead";
                            ship.CompletedOrders.Add(this);
                            Debug.WriteLine(string.Format("Resolving {0}, Target Dead", weapon.Name));
                            ship.CountOfResolvingOrders--;
                        }
                        else
                        {
                            // target is in range
                            if (LocationCollection.GetDistance(ship.Position, target.Position) >= weapon.Range + 1)
                            {
                                result = "Target Out Of Range";
                                Debug.WriteLine(string.Format("Resolving {0}, Target Out Of Range", weapon.Name));
                                ship.CountOfResolvingOrders--;
                            }
                            else
                            {
                                weapon.Target = target;
                                result = weapon.Fire();
                                OnWeaponFired(this, new EventArgs(), ship.Position, target.Position, weapon.FiringType);
                            }
                        }
                    }
                }
            }
            
            this.IsCompleted = true;
            return result;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("WeaponPart", (WeaponPart)this.OrderValues[0]);
            info.AddValue("TargetShip", (Ship)this.OrderValues[1]);
        }

        public override string ToString()
        {
            WeaponPart weapon = (WeaponPart)this.OrderValues[0];
            Ship target = (Ship)this.OrderValues[1];
            return string.Format("Fire {0} at {1}", weapon.Name, target.ClassName);
        }

        public bool IsInRange(Ship ship)
        {
            WeaponPart weapon = (WeaponPart)this.OrderValues[0];
            Ship target = (Ship)this.OrderValues[1];
            return LocationCollection.GetDistance(ship.Position, target.Position) <= weapon.Range+1;
        }

        #region Constructors
        public FireWeaponAtTarget(WeaponPart weapon, Ship targetShip)
        {
            this.OrderValues = new object[2];
            this.OrderValues[0] = weapon;
            this.OrderValues[1] = targetShip;
        }
        public FireWeaponAtTarget(object[] OrderValues)
        {
            this.OrderValues = OrderValues;
        }
        public FireWeaponAtTarget(SerializationInfo info, StreamingContext ctxt)
        {
            this.OrderValues = new object[2];
            this.OrderValues[0] = (WeaponPart)info.GetValue("WeaponPart",typeof(WeaponPart));
            this.OrderValues[1] = (Ship)info.GetValue("TargetShip",typeof(Ship));
        }
        #endregion
    }
}
