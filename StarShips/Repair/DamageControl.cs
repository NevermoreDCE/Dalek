using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;
using StarShips.Randomizer;

namespace StarShips.Repair
{
    [Serializable]
    public class DamageControl: ShipPart, IActionable
    {
        int _repairAmount = 5;
        Ship _target;

        public string DoAction(Ship target)
        {
            if (_target == null)
                _target = target;
            return repair(_target);
        }

        private string repair(Ship target)
        {
            List<string> repaired = new List<string>();
            string result = string.Empty;
            using(RNG rand = new RNG())
            {
                foreach (ShipPart part in target.Equipment.Where(f => f.IsDestroyed))
                    if (rand.d100() > 50)
                    {
                        result = part.Repair(_repairAmount);
                        repaired.Add(part.Name+(!string.IsNullOrEmpty(result)?string.Format(" ({0} HPs)",result):string.Empty));
                    }
            }
            result = string.Format("Repaired {0} for {1} HPs", target.Name, target.HP.Add(_repairAmount).ToString());
            if(repaired.Count>0)
                result = result+string.Format(", and Repaired {0}", string.Join(", ",repaired.ToArray()));

            return result;
        }

        public override string ToString()
        {
            return string.Format("{0} (Regen: {1} HP)", this.Name, _repairAmount);
        }

        public DamageControl()
        {
            this.Name = "Damage Control";
        }

        public override string Repair(int amount)
        {
            this.IsDestroyed = false;
            return string.Empty;
        }
    }
}
