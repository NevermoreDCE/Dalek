using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace StarShips.PartBase
{
    [Serializable]
    public class DefensePart : ShipPart, IDefense, ISerializable
    {
        #region Private Variables
        int _dr;
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

        public void GetObjectXML(XDocument sourceDoc)
        {
            XElement def;

            if (sourceDoc.Descendants("defensePart").Where(f => f.Attribute("name").Value == this.Name).Count() > 0)
            {
                // Update Existing
                def = sourceDoc.Descendants("defensePart").First(f => f.Attribute("name").Value == this.Name);
                def.Element("MaxHP").Value = this.HP.Max.ToString();
                def.Element("DR").Value = this._dr.ToString();
                def.Element("DownAdjective").Value = this._downAdjective.ToString();
                def.Element("PenetrateVerb").Value = this._penetrateVerb.ToString();

                addActions(def.Element("Actions"));
            }
            else
            {
                // Create New
                XElement actions = new XElement("Actions");
                addActions(actions);
                def =
                    new XElement("defensePart", new XAttribute("name", this.Name),
                        new XElement("MaxHP", this.HP.Max.ToString()),
                        new XElement("DR", this._dr.ToString()),
                        new XElement("DownAdjective", this._downAdjective.ToString()),
                        new XElement("PenetrateVerb", this._penetrateVerb.ToString()),
                        actions);
                sourceDoc.Element("defenseParts").Add(def);
            }
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

        public DefensePart(XElement description)
        {
            this.Name = description.Attribute("name").Value;
            this.HP.Max = int.Parse(description.Element("MaxHP").Value);
            this._dr = int.Parse(description.Element("DR").Value);
            this._downAdjective = description.Element("DownAdjective").Value;
            this._penetrateVerb = description.Element("PenetrateVerb").Value;
            loadActions(description.Element("Actions"));
        }
        #endregion

    }
}
