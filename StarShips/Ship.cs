using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;
using StarShips.Randomizer;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace StarShips
{
    [Serializable]
    public class StatWithMax:ISerializable
    {
        int _max = 0;
        int _current = 0;
        public int Max { get { return _max; } set { _max = value; _current = value; } }
        public int Current { get { return _current; } set { _current = value; } }
        public override string ToString()
        {
            return string.Format("{0}/{1}", Current.ToString("D" + Max.ToString().Length.ToString()), Max.ToString());
        }
        public int Add(int amount)
        {
            int result;
            if (amount == int.MaxValue)
            {
                result = Max;
                Current = Max;
            }
            else
            {
                result = Math.Min(Max, Current + amount) - Current;
                Current = Math.Min(Max, Current + amount);
            }
            return result;
        }
        public int Reduce(int amount)
        {
            int result = Math.Max(0, Current - amount) + Current;
            Current = Math.Max(0, Current - amount);
            return result;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Max", Max);
            info.AddValue("Current", Current);
        }

        public StatWithMax(SerializationInfo info, StreamingContext ctxt)
        {
            Max = (int)info.GetValue("Max", typeof(int));
            Current = (int)info.GetValue("Current", typeof(int));
        }

        public StatWithMax()
        {

        }
    }

    [Serializable]
    public class Ship : ISerializable
    {
        #region Properties
        public StatWithMax HP = new StatWithMax();
        public StatWithMax MP = new StatWithMax();
        public List<ShipPart> Equipment = new List<ShipPart>();
        public string Name;
        public int PointCost { get { int result = HP.Max * 5; foreach (ShipPart part in Equipment)result += part.PointCost; return result; } }
        #endregion

        #region Public Methods
        public List<string> FireWeapons(Ship Target)
        {
            List<string> result = new List<string>();

            foreach (IWeapon weapon in Equipment.Where(f => f is IWeapon))
            {
                ShipPart part = (ShipPart)weapon;

                // check if destroyed
                if (part.IsDestroyed)
                    result.Add(string.Format("{0} is destroyed!", part.Name));
                else
                {
                    // check if needs to be reloaded
                    if (!weapon.IsLoaded)
                        result.Add(string.Format("{0} will be reloaded in {1} turns", part.Name, weapon.Reload()));
                    else
                    {
                        // check if has a target
                        if (part.Target == null)
                            part.Target = Target;
                        result.Add(string.Format("{0} fires, {1}", part.Name,weapon.Fire()));
                    }
                }
            }

            return result;
        }

        public List<string> HitFor(int Damage)
        {
            List<string> result = new List<string>();

            foreach (IDefense defense in Equipment.Where(f => f is IDefense && !f.IsDestroyed))
            {
                DefenseResult res = defense.TakeHit(Damage);
                Damage = res.Remainder;
                result.Add(res.Message);
                if (Damage <= 0)
                    break;
            }

            if (Damage > 0)
            {
                HP.Current -= Damage;
                result.Add(string.Format("{0} Damage made it through!", Damage));
                // chance to destroy ship part
                using (RNG rand = new RNG())
                {
                    int countOfEquipment = Equipment.Count*2;
                    int random = rand.d(countOfEquipment);
                    if (random <= Equipment.Count)
                    {
                        if (!Equipment[random-1].IsDestroyed)
                        {
                            Equipment[random-1].IsDestroyed = true;
                            result.Add(string.Format("{0} is destroyed by the damage!", Equipment[random-1].Name));
                        }
                    }

                }
            }

            return result;
        }

        public List<string> EndOfTurn()
        {
            List<string> result = new List<string>();
            foreach (ShipPart part in Equipment.Where(f => !f.IsDestroyed))
            {
                result.Add(part.DoAction(this));
            }
            return result.Where(f=>f!=string.Empty).ToList<string>();


        }

        public override string ToString()
        {
            return string.Format("{0} (HP:{1}/{2})", this.Name, this.HP.Current, this.HP.Max);
        }
        #endregion

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("HP", HP);
            info.AddValue("MP", MP);
            foreach (ShipPart part in Equipment)
                part.Target = null;
            info.AddValue("Equipment", Equipment);
            info.AddValue("Name", Name);
        }

        public void GetObjectXML(XDocument sourceDoc)
        {
            XElement ship;

            if (sourceDoc.Descendants("ship").Where(f => f.Attribute("name").Value == this.Name).Count() > 0)
            {
                // Update Existing
                ship = sourceDoc.Descendants("ship").First(f => f.Attribute("name").Value == this.Name);
                ship.Element("MaxHP").Value = HP.Max.ToString();
                ship.Element("MaxMP").Value = MP.Max.ToString();
                addPartsXML(ship.Element("shipParts"));

            }
            else
            {
                // Create New
                XElement parts = new XElement("shipParts");
                addPartsXML(parts);

                ship =
                    new XElement("ship", new XAttribute("name", this.Name),
                        new XElement("MaxHP", HP.Max.ToString()),
                        new XElement("MaxMP", MP.Max.ToString()),
                        parts);
                sourceDoc.Element("ships").Add(ship);
            }
        }

        private void addPartsXML(XElement shipParts)
        {
            List<XElement> parts = new List<XElement>();
            foreach (var shipPart in this.Equipment)
                parts.Add(new XElement("shipPart", shipPart.Name));
            shipParts.RemoveAll();
            foreach (var element in parts)
                shipParts.Add(element);
        }

        #endregion

        #region Constructors
        public Ship()
        {
            /* Empty Constructor */
        }
        public Ship(SerializationInfo info, StreamingContext ctxt)
        {
            HP = (StatWithMax)info.GetValue("HP", typeof(StatWithMax));
            MP = (StatWithMax)info.GetValue("MP", typeof(StatWithMax));
            Equipment = (List<ShipPart>)info.GetValue("Equipment", typeof(List<ShipPart>));
            Name = (string)info.GetValue("Name", typeof(string));
        }
        public Ship(XElement description, List<ShipPart> partsList)
        {
            this.Name = description.Attribute("name").Value;
            this.HP.Max = int.Parse(description.Element("MaxHP").Value);
            this.MP.Max = int.Parse(description.Element("MaxMP").Value);
            foreach (XElement SPE in description.Element("shipParts").Elements())
                this.Equipment.Add(partsList.First(f => f.Name == SPE.Value).Clone());
        }

        #endregion

    }
}
