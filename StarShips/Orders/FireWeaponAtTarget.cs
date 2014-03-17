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
    [Serializable]
    public class FireWeaponAtTarget: ShipOrder,ITacticalWeaponOrder, ISerializable
    {
        #region Events
        public event OrderDelegates.TacticalWeaponFiredEvent OnWeaponFired;
        #endregion

        public override List<string> ExecuteOrder(Ship ship)
        {
            List<string> result = new List<string>();
            // weapon has subscribed listeners
            if (OnWeaponFired != null)
            {
                Ship target = (Ship)this.OrderValues[1];
                WeaponPart weapon = (WeaponPart)this.OrderValues[0];
                
                // weapon is not destroyed
                if (weapon.IsDestroyed)
                {
                    result.Add(string.Format("{0} is destroyed!", weapon.Name));
                    Debug.WriteLine(string.Format("Resolving {0}, Destroyed", weapon.Name));
                    ship.CompletedOrders.Add(this);
                    this.IsCompleted = true;
                }
                else
                {
                    // weapon is reloaded
                    if (!weapon.IsLoaded)
                    {
                        result.Add(string.Format("{0} will be reloaded in {1} turns", weapon.Name, weapon.Reload()));
                        Debug.WriteLine(string.Format("Resolving {0}, Reloading", weapon.Name));
                        this.IsCompleted = true;
                    }
                    else
                    {

                        // target is valid
                        if (target.HP.Current <= 0)
                        {
                            result.Add("Target Already Dead");
                            ship.CompletedOrders.Add(this);
                            Debug.WriteLine(string.Format("Resolving {0}, Target Dead", weapon.Name));
                            this.IsCompleted = true;
                        }
                        else
                        {
                            // target is in range
                            if (LocationCollection.GetTacticalDistance(ship.TacticalPosition, target.TacticalPosition) >= weapon.Range + 1)
                            {
                                result.Add("Target Out Of Range");
                                Debug.WriteLine(string.Format("Resolving {0}, Target Out Of Range", weapon.Name));
                            }
                            else
                            {
                                weapon.Target = target;
                                result = result.Concat(weapon.Fire()).ToList<string>();
                                OnWeaponFired(this, new EventArgs(), ship.TacticalPosition, target.TacticalPosition, weapon.FiringType);
                                this.IsCompleted = true;
                            }
                        }
                    }
                }
            }
            
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
            return LocationCollection.GetTacticalDistance(ship.TacticalPosition, target.TacticalPosition) <= weapon.Range+1;
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
