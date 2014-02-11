using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Orders.Interfaces;
using StarShips.Parts;
using System.Runtime.Serialization;
using StarShips.Orders.Delegates;

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
            if (OnWeaponFired != null)
            {
                WeaponPart weapon = (WeaponPart)this.OrderValues[0];
                if (weapon.IsDestroyed)
                {
                    result = string.Format("{0} is destroyed!", weapon.Name);
                    ship.CompletedOrders.Add(this);
                }
                else
                {
                    // check if needs to be reloaded
                    if (!weapon.IsLoaded)
                        result = string.Format("{0} will be reloaded in {1} turns", weapon.Name, weapon.Reload());
                    else
                    {
                        Ship target = (Ship)this.OrderValues[1];
                        if (target.HP.Current <= 0)
                        {
                            result = "Target Already Dead";
                            ship.CompletedOrders.Add(this);
                        }
                        else
                        {
                            weapon.Target = target;
                            OnWeaponFired(this, new EventArgs(), ship.Position, target.Position, weapon.FiringType);
                            result = weapon.Fire();
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
            return string.Format("Fire {0} at {1}", weapon.Name, target.Name);
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
