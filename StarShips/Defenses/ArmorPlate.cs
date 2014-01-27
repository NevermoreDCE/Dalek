using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;

namespace StarShips.Defenses
{
    [Serializable]
    public class ArmorPlate:ShipPart,IDefense
    {
        int _dr = 3;
        StatWithMax Hitpoints;
        
        public DefenseResult TakeHit(int Damage)
        {
            DefenseResult result;

            if (Hitpoints.Current <= 0)
            {
                result.Remainder = Damage;
                result.Message = "Armor Plate is Destroyed!";
                return result;
            }

            if (Damage <= -_dr)
            {
                result.Remainder = 0;
                result.Message = "Bounces off Armor Plate for No Damage!";
            }
            else
            {
                int afterDR = Damage - _dr;
                if (afterDR >= Hitpoints.Current)
                {
                    result.Remainder = afterDR - Hitpoints.Current;
                    result.Message = string.Format("Hits Armor Plate for {0}, Destroying It!", Hitpoints.Current);
                    this.IsDestroyed = true;
                    Hitpoints.Current = 0;
                }
                else
                {
                    result.Remainder = 0;
                    result.Message = string.Format("Hits Armor Plate for {0}", afterDR);
                    Hitpoints.Current -= afterDR;
                }
            }


            return result;
        }

        public string Recover()
        {
            return string.Format("Armor Plate, current HP: {0}", Hitpoints.Current);
        }

        public override string ToString()
        {
            return string.Format("{0} (DR:{1}) (HP:{2}/{3})", this.Name, _dr.ToString(), Hitpoints.Current.ToString(), Hitpoints.Max.ToString());
        }

        public ArmorPlate()
        {
            this.Name = "Armor Plate";
            Hitpoints.Max = 15;
            Hitpoints.Current = 15;
        }

        public override string Repair(int amount)
        {
            this.IsDestroyed = false;
            return Hitpoints.Add(amount).ToString();
        }
        public string DoAction(Ship target)
        {
            return string.Empty;
        }
    }
}
