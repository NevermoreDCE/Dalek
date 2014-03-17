using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;
using StarShips.Utility;

namespace StarShips.Parts
{
    [Serializable]
    public class EnginePart : ShipPart, ISerializable
    {
        #region Private Variables
        double _thrust = 0;
        #endregion
        #region Public Properties
        public double Thrust { get { return _thrust; } }
        #endregion
        public override string ToString()
        {
            return string.Format("{0} ({1}kN/s)",this.Name,_thrust.ToString("0.0"));
        }

        #region Serialization
        public override void GetObjectXML(XDocument sourceDoc)
        {
            XElement eng;

            if (sourceDoc.Descendants("enginePart").Where(f => f.Attribute("name").Value == this.Name).Count() > 0)
            {
                // Update Existing
                eng = sourceDoc.Descendants("enginePart").First(f => f.Attribute("name").Value == this.Name);
                eng.Element("MaxHP").Value = this.HP.Max.ToString();
                if (eng.Element("Mass") != null)
                    eng.Element("Mass").Value = this._mass.ToString();
                else
                    eng.Add(new XElement("Mass", this._mass.ToString()));
                eng.Element("Thrust").Value = this._thrust.ToString();
                addActions(eng.Element("Actions"));
            }
            else
            {
                // Create New
                XElement actions = new XElement("Actions");
                addActions(actions);
                eng =
                    new XElement("enginePart", new XAttribute("name", this.Name),
                        new XElement("MaxHP", this.HP.Max.ToString()),
                        new XElement("Mass",this.Mass.ToString()),
                        new XElement("Thrust", this._thrust.ToString()),
                        actions);
                sourceDoc.Descendants("engineParts").First().Add(eng);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("HP", HP);
            info.AddValue("Mass", Mass);
            info.AddValue("Actions", _actions);
            info.AddValue("Thrust", _thrust);
        }
            
        #endregion

        #region Constructors
        public EnginePart(Eidos parent, string Name, int MaxHP, double Mass, double Thrust, List<EidosAction> Actions)
        {
            this._parent = parent;
            this.Name = Name;
            HP.Max = MaxHP;
            HP.Current = MaxHP;
            _mass = Mass;
            _thrust = Thrust;
            _actions = Actions;
        }
        public EnginePart(SerializationInfo info, StreamingContext context)
        {
            this.Name = (string)info.GetValue("Name", typeof(string));
            this.HP = (StatWithMax)info.GetValue("HP", typeof(StatWithMax));
            _mass = (double)info.GetValue("Mass", typeof(double));
            this.Actions = (List<EidosAction>)info.GetValue("Actions", typeof(List<EidosAction>));
            this._thrust = (double)info.GetValue("Thrust", typeof(double));
        }
        public EnginePart(XElement description, Ship parent)
        {
            this._parent = parent;
            this.Name = description.Attribute("name").Value;
            this.HP.Max = int.Parse(description.Element("MaxHP").Value);
            if (description.Element("Mass") != null)
                this._mass = double.Parse(description.Element("Mass").Value);
            this._thrust = double.Parse(description.Element("Thrust").Value);
            loadActions(description.Element("Actions"));
        }
        #endregion
    }
}
