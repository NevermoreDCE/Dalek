using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;

namespace StarShips.Defenses
{
    [Serializable]
    public class ShieldGenerator:ShipPart,IDefense, IActionable
    {
        int _dr = 1;
        StatWithMax Hitpoints;
        Ship _target;

        public DefenseResult TakeHit(int Damage)
        {
            DefenseResult result;
            if (Hitpoints.Current <= 0)
            {
                result.Remainder = Damage;
                result.Message = "Shield Generator is Down!";
                return result;
            }

            if (Damage <= -_dr)
            {
                result.Remainder = 0;
                result.Message = "Bounces off Shield Generator for No Damage!";
            }
            else
            {
                int afterDR = Damage - _dr;
                if (afterDR >= Hitpoints.Current)
                {
                    result.Remainder = afterDR - Hitpoints.Current;
                    result.Message = string.Format("Hits Shield Generator for {0}, Penetrating It!", Hitpoints.Current);
                    Hitpoints.Current = 0;
                }
                else
                {
                    result.Remainder = 0;
                    result.Message = string.Format("Hits Shield Generator for {0}", afterDR);
                    Hitpoints.Current -= afterDR;
                }
            }


            return result;
        }

        private string recover()
        {
            if (Hitpoints.Current < Hitpoints.Max)
            {
                if (Hitpoints.Current <= Hitpoints.Max - 2)
                {
                    Hitpoints.Current += 2;
                    return string.Format("Shield Generator recovered 2, current HP: {0}", Hitpoints.Current);
                }
                else
                {
                    Hitpoints.Current = Hitpoints.Max;
                    return string.Format("Shield Generator, current HP: {0}", Hitpoints.Current);
                }
            }
            return string.Format("Shield Generator, current HP: {0}", Hitpoints.Current);
        }

        public override string ToString()
        {
            return string.Format("{0} (DR:{1}) (HP:{2}/{3})", this.Name, _dr.ToString(), Hitpoints.Current.ToString(), Hitpoints.Max.ToString());
        }

        public ShieldGenerator()
        {
            this.Name = "Shield Generator";
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
            if (_target == null)
                _target = target;
            return recover();
        }
    }
}
