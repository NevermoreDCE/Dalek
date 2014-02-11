using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;
using System.Runtime.Serialization;
using StarShips.Randomizer;

namespace StarShips.Actions
{
    [Serializable]
    public class RepairTargetShip : ShipAction, ISerializable
    {
        #region Public Methods
        public override string DoAction(ShipPart target)
        {
            Ship _target = target.Target;
            List<string> repaired = new List<string>();
            string result = string.Empty;
            using (RNG rand = new RNG())
            {
                foreach (ShipPart part in _target.Equipment.Where(f => f.IsDestroyed))
                    if (rand.d100() > 50)
                    {
                        result = part.Repair((int)ActionValues[0]);
                        repaired.Add(part.Name + (!string.IsNullOrEmpty(result) ? string.Format(" ({0} HPs)", result) : string.Empty));
                    }
            }
            result = string.Format("Repaired {0} for {1} HPs", _target.Name, _target.HP.Add((int)ActionValues[0]).ToString());
            if (repaired.Count > 0)
                result = result + string.Format(", and Repaired {0}", string.Join(", ", repaired.ToArray()));

            return result;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Amount", (int)ActionValues[0]);
        }

        public override string ToString()
        {
            return string.Format("Repair target Ship  for {0} HPs, 50% chance to repair Parts", ((int)ActionValues[0]).ToString());
        }
        #endregion

        #region Constructors
        public RepairTargetShip(int Amount)
        {
            ActionValues[0] = Amount;
        }
        /// <summary>
        /// Instantiates RepairTargetShip with specified ActionValues.
        /// </summary>
        /// <param name="ActionValues">ActionValue[0] will be cast to int</param>
        public RepairTargetShip(object[] ActionValues)
        {
            this.ActionValues = ActionValues;
        }
        public RepairTargetShip(SerializationInfo info, StreamingContext ctxt)
        {
            ActionValues[0] = (int)info.GetValue("Amount", typeof(int));
        }
        #endregion

        
    }
}
