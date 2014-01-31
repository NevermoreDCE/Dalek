using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Utility;
using System.Runtime.Serialization;
using System.Xml.Linq;
using StarShips.PartBase;

namespace StarShips
{
    [Serializable]
    public class PartCount : ISerializable
    {
        public Type PartType;
        public string ActionMechanism = string.Empty;
        public int CountOfParts;

        public override string ToString()
        {
            return string.Format("{0}{1} ({2})",
                PartType.Name,
                (ActionMechanism != string.Empty ? string.Format(" ({0})", ActionMechanism) : string.Empty),
                CountOfParts.ToString());
        }

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PartType", PartType);
            info.AddValue("ActionMechanism", ActionMechanism);
            info.AddValue("CountOfParts", CountOfParts);
        }
        public XElement GetObjectXML()
        {
            XElement result = new XElement("allowedPart",
                new XAttribute("type", PartType.ToString()),
                new XElement("ActionMechanism", ActionMechanism),
                new XElement("CountOfParts", CountOfParts.ToString()));
            return result;
        }
        #endregion

        #region Constructors
        public PartCount(Type partType, string actionMechanism, int countOfParts)
        {
            this.PartType = partType;
            this.ActionMechanism = actionMechanism;
            this.CountOfParts = countOfParts;
        }
        public PartCount(XElement details)
        {
            this.PartType = Type.GetType(details.Attribute("type").Value);
            this.ActionMechanism = details.Element("ActionMechanism").Value;
            this.CountOfParts = int.Parse(details.Element("CountOfParts").Value);
        }
        public PartCount(SerializationInfo info, StreamingContext ctxt)
        {
            this.PartType = (Type)info.GetValue("PartType", typeof(Type));
            this.ActionMechanism = (string)info.GetValue("ActionMechanism", typeof(string));
            this.CountOfParts = (int)info.GetValue("CountOfParts", typeof(int));
        }
        #endregion
    }

    [Serializable]
    public class ShipHull : ISerializable
    {
        #region Private Variables
        string _name = "No Name";
        StatWithMax _hullPoints = new StatWithMax();
        List<PartCount> _allowedParts = new List<PartCount>();
        #endregion

        #region Public Properties
        public string Name
        {
            get
            {
                return _name;
            }
        }
        public StatWithMax HullPoints { get { return _hullPoints; } set { _hullPoints = value; } }
        public List<PartCount> AllowedParts { get { return _allowedParts; } }
        
        #endregion

        #region Public Methods
        public ShipHull Clone()
        {
            ShipHull result;

            ShipHull source = (ShipHull)this;
            List<PartCount> newParts = new List<PartCount>();
            foreach (PartCount oldPart in this._allowedParts)
            {
                newParts.Add(new PartCount(oldPart.PartType,oldPart.ActionMechanism,oldPart.CountOfParts));
            }
            result = new ShipHull(source.Name, source._hullPoints.Max, newParts);
            return result;
        }

        public static List<ShipHull> GetShipHulls(XDocument doc)
        {
            List<ShipHull> result = new List<ShipHull>();

            foreach (XElement hull in doc.Element("shipHulls").Elements())
                result.Add(new ShipHull(hull));

            return result;
        }
        #endregion

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", this._name);
            info.AddValue("HullPoints", this._hullPoints);
            info.AddValue("AllowedParts", this._allowedParts);
        }
        public void GetObjectXML(XDocument doc)
        {
            XElement hull;
            if (doc.Descendants("shipHull").Where(f => f.Attribute("name").Value == this.Name).Count() > 0)
            {
                // Update Existing
                hull = doc.Descendants("shipHull").First(f => f.Attribute("name").Value == this.Name);
                hull.Element("MaxHP").Value = this._hullPoints.Max.ToString();
                AddAllowedParts(hull.Element("AllowedParts"));
            }
            else
            {
                // Create New
                XElement parts = new XElement("AllowedParts");
                AddAllowedParts(parts);
                hull = new XElement("shipHull", new XAttribute("name", this.Name),
                        new XElement("MaxHP", this._hullPoints.Max),
                        parts);
                doc.Element("shipHulls").Add(hull);
            }
            
        }

        private void AddAllowedParts(XElement allowedParts)
        {
            allowedParts.RemoveAll();
            foreach (PartCount pc in _allowedParts)
                allowedParts.Add(pc.GetObjectXML());
        }
        #endregion

        #region Constructors
        public ShipHull()
        {
            /* Empty Constructor */
        }
        public ShipHull(string Name, int MaxHP, List<PartCount> AllowedParts)
        {
            this._name = Name;
            this._hullPoints.Max = MaxHP;
            this._allowedParts = AllowedParts;
        }
        public ShipHull(SerializationInfo info, StreamingContext ctxt)
        {
            this._name = (string)info.GetValue("Name", typeof(string));
            this._hullPoints = (StatWithMax)info.GetValue("HullPoints", typeof(StatWithMax));
            this._allowedParts = (List<PartCount>)info.GetValue("AllowedParts", typeof(List<PartCount>));
        }
        public ShipHull(XElement description)
        {
            this._name = description.Attribute("name").Value;
            this._hullPoints.Max = int.Parse(description.Element("MaxHP").Value);
            foreach (var part in description.Element("AllowedParts").Elements())
            {
                this._allowedParts.Add(new PartCount(part));
            }
        }
        #endregion

        
    }
}
