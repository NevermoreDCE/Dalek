using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;
using StarShips.Randomizer;
using System.Runtime.Serialization;
using System.Xml.Linq;
using StarShips.Utility;
using System.Drawing;
using StarShips.Delegates;
using StarShips.Parts;
using StarShips.Orders.Interfaces;
using System.Diagnostics;


namespace StarShips
{
    [Serializable]
    public class Ship : ISerializable
    {
        #region Events
        public event ShipDelegates.ShipDestroyedEvent OnShipDestroyed;
        #endregion

        #region Properties
        public StatWithMax HP { get { return HullType.HullPoints; } set { HullType.HullPoints = value; } }
        public StatWithMax MP = new StatWithMax();
        public List<ShipPart> Equipment = new List<ShipPart>();
        public List<ShipOrder> Orders = new List<ShipOrder>();
        public List<ShipOrder> CompletedOrders = new List<ShipOrder>();
        public string Name;
        public int PointCost { get { int result = HP.Max * 5; foreach (ShipPart part in Equipment)result += part.PointCost; return result; } }
        public ShipHull HullType = new ShipHull();
        public Point Position = new Point(-1, -1);
        public Point Origin;
        public System.Windows.Controls.Image Image;
        public int CountOfResolvingOrders = 0;
        bool _weaponsFiredAlready = false;
        public bool WeaponsFiredAlready { get { return _weaponsFiredAlready; } }
        #endregion

        #region Public Methods
        public List<string> FireWeapons(Ship Target)
        {
            List<string> result = new List<string>();

            foreach (WeaponPart weapon in Equipment.Where(f => f is WeaponPart))
            {
                // check if destroyed
                if (weapon.IsDestroyed)
                    result.Add(string.Format("{0} is destroyed!", weapon.Name));
                else
                {
                    // check if needs to be reloaded
                    if (!weapon.IsLoaded)
                        result.Add(string.Format("{0} will be reloaded in {1} turns", weapon.Name, weapon.Reload()));
                    else
                    {
                        // check if has a target
                        if (weapon.Target == null)
                            weapon.Target = Target;
                        result.Add(string.Format("{0} fires, {1}", weapon.Name,weapon.Fire()));
                    }
                }
            }

            return result;
        }

        public List<string> HitFor(int Damage)
        {
            return HitFor(Damage, string.Empty);
        }
        public List<string> HitFor(int Damage, string DamageType)
        {
            List<string> result = new List<string>();

            foreach (DefensePart defense in Equipment.Where(f => f is DefensePart && !f.IsDestroyed))
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
                    int countOfEquipment = Equipment.Count * 2;
                    int random = rand.d(countOfEquipment);
                    if (random <= Equipment.Count)
                    {
                        if (!Equipment[random - 1].IsDestroyed)
                        {
                            Equipment[random - 1].IsDestroyed = true;
                            result.Add(string.Format("{0} is destroyed by the damage!", Equipment[random - 1].Name));
                        }
                    }

                }
                if(HP.Current<=0)
                    if (OnShipDestroyed != null)
                    {
                        OnShipDestroyed(this, new EventArgs(), this);
                    }
            }

            return result;
        }

        public List<string> ExecuteOrders(int Impulse)
        {
            List<string> result = new List<string>();
            
            foreach (ShipOrder order in Orders.Where(f=>f.Impulse==Impulse))
                result.Add(order.ExecuteOrder(this));

            // clear up completed orders
            if (CompletedOrders.Count > 0)
            {
                foreach (ShipOrder order in CompletedOrders)
                    Orders.Remove(order);
                CompletedOrders.Clear();
            }

            return result.Where(f => f != string.Empty).ToList<string>();
        }

        public List<string> StartOfTurn()
        {
            foreach (ShipOrder order in Orders)
                order.IsCompleted = false;
            return new List<string>();
        }

        public List<string> oldStartOfTurn()
        {
            
            // refresh orders for this turn
            foreach(ShipOrder order in Orders)
                order.IsCompleted = false;
            
            List<string> results = new List<string>();
            
            List<ShipOrder> afterMovement = new List<ShipOrder>();
            // resolve weapon orders that are in range
            Debug.WriteLine(string.Format("Firing First, count of orders {0}",CountOfResolvingOrders));
            foreach(ShipOrder fireOrder in Orders.Where(f=>f is IWeaponOrder))
            {
                if (((IWeaponOrder)fireOrder).IsInRange(this))
                {
                    Debug.WriteLine(string.Format("{0} is in range, firing", ((WeaponPart)fireOrder.OrderValues[0]).Name));
                    results.Add(fireOrder.ExecuteOrder(this));
                    this.CountOfResolvingOrders++;
                    _weaponsFiredAlready = true;
                }
                else
                    afterMovement.Add(fireOrder);
            }
            Debug.WriteLine(string.Format("Done Firing First, count of orders {0}",CountOfResolvingOrders));
            
            // wait for orders to resolve
            //while (CountOfResolvingOrders > 0)
            //{
            //}

            Debug.WriteLine(string.Format("Moving, count of orders {0}",CountOfResolvingOrders));
            // non-weapon orders
            foreach (ShipOrder order in Orders.Where(f => !(f is IWeaponOrder)))
            {
                Debug.WriteLine("Engaging Move Order");
                results.Add(order.ExecuteOrder(this));
                this.CountOfResolvingOrders++;
            }
            Debug.WriteLine(string.Format("Done Moving, count of orders {0}",CountOfResolvingOrders));
            
            //wait for orders to resolve
            //while (CountOfResolvingOrders > 0)
            //{
            //}

            Debug.WriteLine(string.Format("Firing Second, count of orders {0}",CountOfResolvingOrders));
            
            // retry weapon orders that were out of range
            foreach (ShipOrder fireOrder in afterMovement)
            {
                if (((IWeaponOrder)fireOrder).IsInRange(this))
                {
                    results.Add(fireOrder.ExecuteOrder(this));
                    this.CountOfResolvingOrders++;
                }
            }
            Debug.WriteLine(string.Format("Done Firing Second, count of orders {0}",CountOfResolvingOrders));
            

            //while (CountOfResolvingOrders > 0)
            //{ }

            // clear up completed orders
            if (CompletedOrders.Count > 0)
            {
                foreach (ShipOrder order in CompletedOrders)
                    Orders.Remove(order);
                CompletedOrders.Clear();
            }

            return results.Where(f => f != string.Empty).ToList<string>();
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
            return string.Format("{0}{1} (HP:{2}/{3})",
                this.Name,
                (this.HullType.Name != string.Empty ? string.Format(" ({0})", this.HullType.Name) : string.Empty),
                this.HP.Current,
                this.HP.Max);
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
            info.AddValue("HullType", HullType);
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
                if (ship.Element("ShipHull") == null)
                    ship.Add(new XElement("ShipHull", this.HullType.Name));
                else
                    ship.Element("ShipHull").Value = this.HullType.Name;
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
                        new XElement("ShipHull",this.HullType.Name),
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
            HullType = (ShipHull)info.GetValue("HullType", typeof(ShipHull));
        }
        public Ship(XElement description, List<ShipPart> partsList)
        {
            this.Name = description.Attribute("name").Value;
            this.HP.Max = int.Parse(description.Element("MaxHP").Value);
            this.MP.Max = int.Parse(description.Element("MaxMP").Value);
            foreach (XElement SPE in description.Element("shipParts").Elements())
                this.Equipment.Add(partsList.First(f => f.Name == SPE.Value).Clone());
        }
        public Ship(XElement description, List<ShipPart> partsList, List<ShipHull> hullsList)
        {
            this.Name = description.Attribute("name").Value;
            this.HP.Max = int.Parse(description.Element("MaxHP").Value);
            this.MP.Max = int.Parse(description.Element("MaxMP").Value);
            foreach (XElement SPE in description.Element("shipParts").Elements())
                this.Equipment.Add(partsList.First(f => f.Name == SPE.Value).Clone());
            if (description.Element("ShipHull") != null)
                if (hullsList.Where(f => f.Name == description.Element("ShipHull").Value).Count() > 0)
                    this.HullType = hullsList.First(f => f.Name == description.Element("ShipHull").Value).Clone();
        }

        #endregion

    }
}
