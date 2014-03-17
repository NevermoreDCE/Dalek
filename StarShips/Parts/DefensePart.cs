using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;
using System.Runtime.Serialization;
using System.Xml.Linq;
using StarShips.Utility;

namespace StarShips.Parts
{
    [Serializable]
    public class DefensePart : ShipPart, ISerializable
    {
        #region Private Variables
        int _dr;
        string _downAdjective = "Down";
        string _penetrateVerb = "Penetrating";
        #endregion

        #region Public Properties
        public int DR { get { return _dr; } }
        public string DownAdjective { get { return _downAdjective; } }
        public string PenetrateVerb { get { return _penetrateVerb; } }
        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        public override string ToString()
        {
            return string.Format("{0} (DR:{1}) (HP:{2}/{3})", this.Name, _dr.ToString(), HP.Current.ToString(), HP.Max.ToString());
        }
        /// <summary>
        /// Handles incoming damage
        /// </summary>
        /// <param name="Damage">Amount of incoming damage</param>
        /// <returns>Status result and remaining damage</returns>
        public DefenseResult TakeHit(int Damage)
        {
            DefenseResult result = new DefenseResult();
            if (HP.Current <= 0)
            {
                result.Remainder = Damage;
                result.Messages.Add(string.Format("{0} is {1}!", this.Name, _downAdjective));
                return result;
            }

            if (Damage <= _dr)
            {
                result.Remainder = 0;
                result.Messages.Add(string.Format("Bounces off {0} for No Damage!",this.Name));
            }
            else
            {
                int afterDR = Damage - _dr;
                if (afterDR >= HP.Current)
                {
                    result.Remainder = afterDR - HP.Current;
                    result.Messages.Add(string.Format("Hits {0} for {1}{2}, {3} It!",
                        this.Name,
                        HP.Current,
                        (_dr>0?string.Format("(DR: {0})",_dr):string.Empty),
                        _penetrateVerb));
                    HP.Current = 0;
                }
                else
                {
                    result.Remainder = 0;
                    result.Messages.Add(string.Format("Hits {0} for {1}{2}", this.Name, afterDR,(_dr > 0 ? string.Format("(DR: {0})", _dr) : string.Empty)));
                    HP.Current -= afterDR;
                }
            }

            return result;
        }
        #endregion

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Parent", _parent);
            info.AddValue("Name", Name);
            info.AddValue("Mass", Mass);
            info.AddValue("HP", HP);
            info.AddValue("DR", _dr);
            info.AddValue("DownAdjective", _downAdjective);
            info.AddValue("PenetrateVerb", _penetrateVerb);
            info.AddValue("Actions", _actions);
        }

        public override void GetObjectXML(XDocument sourceDoc)
        {
            XElement def;

            if (sourceDoc.Descendants("defensePart").Where(f => f.Attribute("name").Value == this.Name).Count() > 0)
            {
                // Update Existing
                def = sourceDoc.Descendants("defensePart").First(f => f.Attribute("name").Value == this.Name);
                def.Element("MaxHP").Value = this.HP.Max.ToString();
                if (def.Element("Mass") != null)
                    def.Element("Mass").Value = this._mass.ToString();
                else
                    def.Add(new XElement("Mass", this._mass.ToString()));
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
                        new XElement("Mass",this._mass.ToString()),
                        new XElement("DR", this._dr.ToString()),
                        new XElement("DownAdjective", this._downAdjective.ToString()),
                        new XElement("PenetrateVerb", this._penetrateVerb.ToString()),
                        actions);
                sourceDoc.Descendants("defenseParts").First().Add(def);
            }
        }

        #endregion

        #region Constructors
        public DefensePart(Eidos parent, string Name, int MaxHP, double Mass, int DR, string DownAdjective, string PenetrateVerb, List<EidosAction> Actions)
        {
            this._parent = parent;
            this.Name = Name;
            HP.Max = MaxHP;
            HP.Current = MaxHP;
            _mass = Mass;
            _dr = DR;
            _downAdjective = DownAdjective;
            _penetrateVerb = PenetrateVerb;
            _actions = Actions;
        }

        public DefensePart(SerializationInfo info, StreamingContext ctxt)
        {
            _parent = (Ship)info.GetValue("Parent", typeof(Ship));
            Name = (string)info.GetValue("Name", typeof(string));
            _dr = (int)info.GetValue("DR", typeof(int));
            HP = (StatWithMax)info.GetValue("HP", typeof(StatWithMax));
            _mass = (double)info.GetValue("Mass", typeof(double));
            _downAdjective = (string)info.GetValue("DownAdjective", typeof(string));
            _penetrateVerb = (string)info.GetValue("PenetrateVerb", typeof(string));
            _actions = (List<EidosAction>)info.GetValue("Actions", typeof(List<EidosAction>));
        }

        public DefensePart(XElement description, Ship parent)
        {
            this._parent = parent;
            this.Name = description.Attribute("name").Value;
            this.HP.Max = int.Parse(description.Element("MaxHP").Value);
            if (description.Element("Mass") != null)
                this._mass = double.Parse(description.Element("Mass").Value);
            this._dr = int.Parse(description.Element("DR").Value);
            this._downAdjective = description.Element("DownAdjective").Value;
            this._penetrateVerb = description.Element("PenetrateVerb").Value;
            loadActions(description.Element("Actions"));
        }
        #endregion

    }
}
