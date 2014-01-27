using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;
using System.Runtime.Serialization;

namespace StarShips.PartBase
{
    [Serializable]
    public class DefensePart : ShipPart, IDefense, ISerializable
    {
        #region Private Variables
        int _dr;
        Ship _target;
        string _downAdjective = "Down";
        string _penetrateVerb = "Penetrating";
        #endregion

        #region Public Properties
        /* Empty Region */
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        public override string ToString()
        {
            return string.Format("{0} (DR:{1}) (HP:{2}/{3})", this.Name, _dr.ToString(), HP.Current.ToString(), HP.Max.ToString());
        }

        public override string Repair(int amount)
        {
            this.IsDestroyed = false;
            return HP.Add(amount).ToString();
        }

        public DefenseResult TakeHit(int Damage)
        {
            DefenseResult result;
            if (HP.Current <= 0)
            {
                result.Remainder = Damage;
                result.Message = string.Format("{0} is {1}!", this.Name, _downAdjective); ;
                return result;
            }

            if (Damage <= -_dr)
            {
                result.Remainder = 0;
                result.Message = string.Format("Bounces off {0} for No Damage!",this.Name);
            }
            else
            {
                int afterDR = Damage - _dr;
                if (afterDR >= HP.Current)
                {
                    result.Remainder = afterDR - HP.Current;
                    result.Message = string.Format("Hits {0} for {1}, {2} It!", this.Name, HP.Current, _penetrateVerb);
                    HP.Current = 0;
                }
                else
                {
                    result.Remainder = 0;
                    result.Message = string.Format("Hits {0} for {1}",this.Name, afterDR);
                    HP.Current -= afterDR;
                }
            }

            return result;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("HP", HP);
            info.AddValue("DR", _dr);
            info.AddValue("DownAdjective", _downAdjective);
            info.AddValue("PenetrateVerb", _penetrateVerb);
            info.AddValue("Actions", _actions);
        }
        #endregion

        #region Constructors
        public DefensePart(string Name, int MaxHP, int DR, string DownAdjective, string PenetrateVerb, List<IShipPartAction> Actions)
        {
            this.Name = Name;
            HP.Max = MaxHP;
            HP.Current = MaxHP;
            _dr = DR;
            _downAdjective = DownAdjective;
            _penetrateVerb = PenetrateVerb;
            _actions = Actions;
        }

        public DefensePart(SerializationInfo info, StreamingContext ctxt)
        {
            Name = (string)info.GetValue("Name", typeof(string));
            _dr = (int)info.GetValue("DR", typeof(int));
            HP = (StatWithMax)info.GetValue("HP", typeof(StatWithMax));
            _downAdjective = (string)info.GetValue("DownAdjective", typeof(string));
            _penetrateVerb = (string)info.GetValue("PenetrateVerb", typeof(string));
            _actions = (List<IShipPartAction>)info.GetValue("Actions", typeof(List<IShipPartAction>));
        }
        #endregion

    }
}
