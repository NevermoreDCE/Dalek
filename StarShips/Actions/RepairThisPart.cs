using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;
using System.Runtime.Serialization;

namespace StarShips.Actions
{
    [Serializable]
    public class RepairThisPart : IShipPartAction, ISerializable
    {
        #region Private Variables
        public int[] ActionValues = new int[1];
        #endregion

        #region Public Methods
        public string DoAction(ShipPart target)
        {
            if (target.HP.Current < target.HP.Max)
            {
                if (target.HP.Current <= target.HP.Max - ActionValues[0])
                {
                    target.HP.Current += ActionValues[0];
                    return string.Format("{0} recovered {1}, current HP: {2}", target.Name, ActionValues[0], target.HP.Current);
                }
                else
                {
                    target.HP.Current = target.HP.Max;
                    return string.Format("{0}, current HP: {0}",target.Name, target.HP.Current);
                }
            }
            return string.Format("{0}, current HP: {0}",target.Name, target.HP.Current);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Amount", ActionValues[0]);
        }

        public override string ToString()
        {
            return string.Format("Repair this Part for {0} HPs", ActionValues[0].ToString());
        }
        #endregion

        #region Constructors
        public RepairThisPart(int amount)
        {
            ActionValues[0] = amount;
        }

        public RepairThisPart(SerializationInfo info, StreamingContext ctxt)
        {
            ActionValues[0] = (int)info.GetValue("Amount", typeof(int));
        }
        #endregion

    }
}
