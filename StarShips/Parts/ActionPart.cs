using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using StarShips.Interfaces;
using System.Xml.Linq;
using StarShips.Utility;

namespace StarShips.Parts
{
    [Serializable]
    public class ActionPart : ShipPart, ISerializable
    {
        #region Private Variables
        string _actionDescription = string.Empty;
        #endregion

        #region Public Properties
        public string Description { get { return _actionDescription; } }
        #endregion

        #region Public Methods
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, _actionDescription);
        }

        #endregion

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("HP", HP);
            info.AddValue("Mass", Mass);
            info.AddValue("ActionDescription", _actionDescription);
            info.AddValue("Actions", _actions);
            info.AddValue("Parent", _parent);
        }

        public override void GetObjectXML(XDocument sourceDoc)
        {
            XElement act;

            if (sourceDoc.Descendants("actionPart").Where(f => f.Attribute("name").Value == this.Name).Count() > 0)
            {
                // Update Existing
                act = sourceDoc.Descendants("actionPart").First(f => f.Attribute("name").Value == this.Name);
                act.Element("MaxHP").Value = this.HP.Max.ToString();
                if (act.Element("Mass") != null)
                    act.Element("Mass").Value = this._mass.ToString();
                else
                    act.Add(new XElement("Mass", this._mass.ToString()));
                act.Element("ActionDescription").Value = this._actionDescription.ToString();
                
                addActions(act.Element("Actions"));
            }
            else
            {
                // Create New
                XElement actions = new XElement("Actions");
                addActions(actions);
                act =
                    new XElement("actionPart", new XAttribute("name", this.Name),
                        new XElement("MaxHP", this.HP.Max.ToString()),
                        new XElement("Mass",this.Mass.ToString()),
                        new XElement("ActionDescription", this._actionDescription.ToString()),
                        actions);
                sourceDoc.Descendants("actionParts").First().Add(act);
            }
        }
        #endregion

        #region Constructors
        public ActionPart(Eidos Parent, string Name, int MaxHP, double Mass, string ActionDescription, List<EidosAction> Actions)
        {
            this._parent = Parent;
            this.Name = Name;
            HP.Max = MaxHP;
            HP.Current = MaxHP;
            _mass = Mass;
            _actionDescription = ActionDescription;
            _actions = Actions;
        }

        public ActionPart(SerializationInfo info, StreamingContext ctxt)
        {
            _parent = (Ship)info.GetValue("Parent", typeof(Ship));
            Name = (string)info.GetValue("Name", typeof(string));
            HP = (StatWithMax)info.GetValue("HP", typeof(StatWithMax));
            _mass = (double)info.GetValue("Mass", typeof(double));
            _actionDescription = (string)info.GetValue("ActionDescription", typeof(string));
            _actions = (List<EidosAction>)info.GetValue("Actions", typeof(List<EidosAction>));
        }

        public ActionPart(XElement description, Ship parent)
        {
            this._parent = parent;
            this.Name = description.Attribute("name").Value;
            this.HP.Max = int.Parse(description.Element("MaxHP").Value);
            if (description.Element("Mass") != null)
                this._mass = double.Parse(description.Element("Mass").Value);
            this._actionDescription = description.Element("ActionDescription").Value;
            loadActions(description.Element("Actions"));
        }
        #endregion

    }
}
