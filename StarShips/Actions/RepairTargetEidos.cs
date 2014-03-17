using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;
using System.Runtime.Serialization;
using StarShips.Randomizer;
using StarShips.Actions.Interfaces;

namespace StarShips.Actions
{
    [Serializable]
    public class RepairTargetEidos : EidosAction, ISerializable, ITargetableAction
    {
        #region Public Methods
        public override string DoAction(EidosPart targetPart)
        {
            Eidos target = targetPart.Target;
            List<string> repaired = new List<string>();
            string result = string.Empty;
            using (RNG rand = new RNG())
            {
                foreach (EidosPart part in target.Parts.Where(f => f.IsDestroyed))
                    if (rand.d100() > 50)
                    {
                        result = part.Repair((int)ActionValues[0]);
                        repaired.Add(part.Name + (!string.IsNullOrEmpty(result) ? string.Format(" ({0} HPs)", result) : string.Empty));
                    }
            }
            if (target is Ship)
            {
                result = string.Format("Repaired {0} for {1} HPs", target.Name, ((Ship)target).HP.Add((int)ActionValues[0]).ToString());
                if (repaired.Count > 0)
                    result = result + string.Format(", and Repaired {0}", string.Join(", ", repaired.ToArray()));
            }
            else
                result = string.Format("Repaired {0}", string.Join(", ", repaired.ToArray()));
            return result;
        }

        public string SetTarget(ShipPart targetPart, Ship targetShip)
        {
            targetPart.Target = targetShip;
            return string.Format("Set {0} target to {1}", targetPart.Name, targetShip.Name);
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
        public RepairTargetEidos(int Amount)
        {
            ActionValues[0] = Amount;
        }
        /// <summary>
        /// Instantiates RepairTargetEidos with specified ActionValues.
        /// </summary>
        /// <param name="ActionValues">ActionValues[0] will be cast to int as the amount to repair</param>
        public RepairTargetEidos(object[] ActionValues)
        {
            this.ActionValues = ActionValues;
        }
        public RepairTargetEidos(SerializationInfo info, StreamingContext ctxt)
        {
            ActionValues[0] = (int)info.GetValue("Amount", typeof(int));
        }
        #endregion

        
    }
}
