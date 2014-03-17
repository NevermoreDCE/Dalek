using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Utility;
using System.Runtime.Serialization;
using System.Xml.Linq;
using StarShips.Parts;

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
                new XElement("CountOfParts", CountOfParts.ToString())
                );
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
        string _imageURL = string.Empty;
        StatWithMax _hullPoints = new StatWithMax();
        List<PartCount> _allowedParts = new List<PartCount>();
        double _mass = 0.0d;
        
        #endregion

        #region Public Properties
        public string Name { get { return _name; } }
        public StatWithMax HullPoints { get { return _hullPoints; } set { _hullPoints = value; } }
        public List<PartCount> AllowedParts { get { return _allowedParts; } }
        public string ImageURL { get { return _imageURL; } }
        public double Mass { get { return _mass; } }
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
            result = new ShipHull(source.Name, source._hullPoints.Max, source.Mass, newParts, source.ImageURL);
            return result;
        }

        public static List<ShipHull> GetShipHulls(XDocument doc)
        {
            List<ShipHull> result = new List<ShipHull>();

            foreach (XElement hull in doc.Element("shipHulls").Elements())
                result.Add(new ShipHull(hull));

            return result;
        }
        public static int HullComparer(ShipHull a, ShipHull b)
        {
            if (a.Mass > b.Mass)
                return 1;
            else if (b.Mass > a.Mass)
                return -1;
            else
                return 0;
        }
        #endregion

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", this._name);
            info.AddValue("HullPoints", this._hullPoints);
            info.AddValue("Mass", this._mass);
            info.AddValue("AllowedParts", this._allowedParts);
            info.AddValue("ImageURL", this._imageURL);
        }
        public void GetObjectXML(XDocument doc)
        {
            XElement hull;
            if (doc.Descendants("shipHull").Where(f => f.Attribute("name").Value == this.Name).Count() > 0)
            {
                // Update Existing
                hull = doc.Descendants("shipHull").First(f => f.Attribute("name").Value == this.Name);
                hull.Element("MaxHP").Value = this._hullPoints.Max.ToString();
                if (hull.Element("Mass") != null)
                    hull.Element("Mass").Value = this._mass.ToString();
                else
                    hull.Add(new XElement("Mass", this._mass.ToString()));
                AddAllowedParts(hull.Element("AllowedParts"));
                if (hull.Element("ImageURL") == null)
                    hull.Add(new XElement("ImageURL"), this._imageURL);
                else
                    hull.Element("ImageURL").Value = this._imageURL;
            }
            else
            {
                // Create New
                XElement parts = new XElement("AllowedParts");
                AddAllowedParts(parts);
                hull = new XElement("shipHull", new XAttribute("name", this.Name),
                        new XElement("MaxHP", this._hullPoints.Max),
                        new XElement("Mass",this._mass.ToString()),
                        parts,
                        new XElement("ImageURL",this._imageURL));
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
        public ShipHull(string Name, int MaxHP, double Mass, List<PartCount> AllowedParts)
        {
            new ShipHull(Name, MaxHP, Mass, AllowedParts, string.Empty);
        }
        public ShipHull(string Name, int MaxHP, double Mass, List<PartCount> AllowedParts, string ImageURL)
        {
            this._name = Name;
            this._hullPoints.Max = MaxHP;
            this._mass = Mass;
            this._allowedParts = AllowedParts;
            this._imageURL = ImageURL;
        }
        public ShipHull(SerializationInfo info, StreamingContext ctxt)
        {
            this._name = (string)info.GetValue("Name", typeof(string));
            this._hullPoints = (StatWithMax)info.GetValue("HullPoints", typeof(StatWithMax));
            this._mass = (double)info.GetValue("Mass", typeof(double));
            this._allowedParts = (List<PartCount>)info.GetValue("AllowedParts", typeof(List<PartCount>));
            this._imageURL = (string)info.GetValue("ImageURL", typeof(string));
        }
        public ShipHull(XElement description)
        {
            this._name = description.Attribute("name").Value;
            this._hullPoints.Max = int.Parse(description.Element("MaxHP").Value);
            if (description.Element("Mass") != null)
                this._mass = double.Parse(description.Element("Mass").Value);
            foreach (var part in description.Element("AllowedParts").Elements())
            {
                this._allowedParts.Add(new PartCount(part));
            }
            if(description.Element("ImageURL")!=null)
                this._imageURL = description.Element("ImageURL").Value;
        }
        #endregion

        
    }
}
