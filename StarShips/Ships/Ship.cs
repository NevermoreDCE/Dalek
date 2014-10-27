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
using System.Windows.Media;
using StarShips.Players;
using System.Xml.Serialization;


namespace StarShips
{
    [Serializable]
    public class Ship : Eidos, ISerializable
    {
        #region Events
        public event ShipDelegates.ShipDestroyedEvent OnShipDestroyed;
        #endregion

        #region Private Variables
        StatWithMax _mp = new StatWithMax();
        string _className;
        ShipHull _hullType = new ShipHull();
        bool _weaponsFiredAlready = false;
        #endregion

        #region Public Properties
        public StatWithMax HP { get { return HullType.HullPoints; } set { HullType.HullPoints = value; } }
        public StatWithMax MP { get { return _mp; } set { _mp = value; } }
        public int ImpulseMultiplier { get { return 30 / (_mp.Max - 1); } }
        public List<ShipOrder> Orders = new List<ShipOrder>();
        public List<ShipOrder> CompletedOrders = new List<ShipOrder>();
        public string ClassName { get { return _className; } set { _className = value; } }
        public int PointCost { get { int result = HP.Max * 5; foreach (ShipPart part in Parts)result += part.PointCost; return result; } }
        public ShipHull HullType { get { return _hullType; } set { _hullType = value; } }
        public Point Origin;
        public override System.Windows.Controls.Image Image { 
            get 
            {
                if (_image == null)
                {
                    System.Windows.Media.Imaging.BitmapImage src = new System.Windows.Media.Imaging.BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri(string.Format("Empires\\{0}\\Images\\{1}", _owner.IconSet, this.HullType.ImageURL), UriKind.Relative);
                    src.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    src.EndInit();
                    initImage((ImageSource)src,40);
                }
                return _image; 
            } set { _image = value; } }

        public int CountOfResolvingOrders = 0;
        public bool WeaponsFiredAlready { get { return _weaponsFiredAlready; } }
        public double TotalMass
        {
            get
            {
                double mass = HullType.Mass;
                foreach (var part in Parts.Where(f=>!f.IsDestroyed))
                    mass += part.Mass;
                foreach (var part in Parts.Where(f => f.IsDestroyed))
                    mass += part.Mass * .25;
                return mass;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Calculates maximum MP for current ship status based on Total Thrust from Engines and Total Mass from Hull and Parts
        /// </summary>
        /// <returns>Number of MP currently possible</returns>
        public int GetMaxMP()
        {
            
            double totalThrust = 0;
            foreach (EnginePart part in this.Parts.Where(f => f is EnginePart))
                totalThrust += part.Thrust;
            if (totalThrust <= 0)
                return 0;

            double SizeCategory = Math.Log((TotalMass / 50), 4);

            double EngineCount = totalThrust / 100;

            double MP = (-0.0332 * SizeCategory + 0.405) * EngineCount + (-0.29093 * SizeCategory + 1.7867);

            return Convert.ToInt32(Math.Round(MP));
        }

        [Obsolete("Use ShipOrders to fire weapons")]
        public List<string> FireWeapons(Ship Target)
        {
            List<string> result = new List<string>();

            foreach (WeaponPart weapon in Parts.Where(f => f is WeaponPart))
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

        /// <summary>
        /// Handles incoming damage to the Ship through any equipped DefenseParts
        /// </summary>
        /// <param name="Damage">Amount of damage to take</param>
        /// <returns>List of status results from DefenseParts</returns>
        public List<string> HitFor(int Damage)
        {
            return HitFor(Damage, string.Empty);
        }
        /// <summary>
        /// Handles incoming damage to teh Ship through any equipped DefenseParts, including resistance based on damage type
        /// </summary>
        /// <param name="Damage">Amount of damage to take</param>
        /// <param name="DamageType">Type of damage to take (Data Driven)</param>
        /// <returns>List of status results from DefenseParts</returns>
        public List<string> HitFor(int Damage, string DamageType)
        {
            List<string> result = new List<string>();

            foreach (DefensePart defense in Parts.Where(f => f is DefensePart && !f.IsDestroyed))
            {
                DefenseResult res = defense.TakeHit(Damage);
                Damage = res.Remainder;
                foreach (string message in res.Messages)
                    result.Add(string.Format(" ==> {0}",message));
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
                    int countOfEquipment = Parts.Count * 2;
                    int random = rand.d(countOfEquipment);
                    if (random <= Parts.Count)
                    {
                        if (!Parts[random - 1].IsDestroyed)
                        {
                            Parts[random - 1].IsDestroyed = true;
                            result.Add(string.Format("{0} is destroyed by the damage!", Parts[random - 1].Name));
                        }
                    }

                }
                if (HP.Current <= 0)
                {
                    if (OnShipDestroyed != null)
                    {
                        OnShipDestroyed(this, new EventArgs(), this);
                    }
                    _isDestroyed = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Executes Orders for a given Impulse value. 
        /// Move orders are only executed if they are evenly divisible by ImpulseMultiplier;
        /// Weapon orders that have not been previously completed are always checked for range and fired if in range;
        /// Completed orders are cleared.
        /// </summary>
        /// <param name="Impulse">The Impulse number currently being executed</param>
        /// <returns>List of status results from Orders</returns>
        public List<string> ExecuteTacticalOrders(int Impulse)
        {
            List<string> result = new List<string>();
            if (this.Orders.Count > 0)
            {
                // move order
                if (Impulse % this.ImpulseMultiplier == 0)
                {
                    ITacticalMoveOrder moveOrder = (ITacticalMoveOrder)Orders.First(f => f is ITacticalMoveOrder);
                    result = result.Concat(moveOrder.ExecuteOrder(this, Impulse)).ToList<string>();
                }
                // check weapon orders
                foreach (ITacticalWeaponOrder order in Orders.Where(f => f is ITacticalWeaponOrder && !f.IsCompleted))
                    if (order.IsInRange(this))
                        result = result.Concat(order.ExecuteOrder(this)).ToList<string>();
                // other orders
                //implement later on

                // clear up completed orders
                if (CompletedOrders.Count > 0)
                {
                    foreach (ShipOrder order in CompletedOrders)
                        Orders.Remove(order);
                    CompletedOrders.Clear();
                }

            }
            return result.Where(f => f != string.Empty).ToList<string>();
        }

        public List<string> ExecuteStrategicOrders()
        {
            List<string> result = new List<string>();
            if (this.Orders.Count > 0)
            {
                // move orders
                while (this.MP.Current > 0 && this.Orders.Where(f => f is IStrategicMoveOrder && !CompletedOrders.Contains(f)).Count() > 0)
                {
                    IStrategicMoveOrder moveOrder = (IStrategicMoveOrder)Orders.First(f => f is IStrategicMoveOrder && !CompletedOrders.Contains(f));
                    result = result.Concat(moveOrder.ExecuteOrder(this)).ToList<string>();
                }
                // other orders
                //implement later on

                // clear up completed orders
                if (CompletedOrders.Count > 0)
                {
                    foreach (ShipOrder order in CompletedOrders)
                        Orders.Remove(order);
                    CompletedOrders.Clear();
                }

            }
            return result.Where(f => f != string.Empty).ToList<string>();
        }

        /// <summary>
        /// Resets IsCompleted status on orders and resets MP value to maximum
        /// </summary>
        public void StartOfTurn()
        {
            foreach (ShipOrder order in Orders)
                order.IsCompleted = false;
            this.MP.Max = this.GetMaxMP();
            this.MP.Current = this.MP.Max;
        }

        /// <summary>
        /// Processes Actions on non-destroyed Parts, and cleans up invalid Weapon/Move Orders
        /// </summary>
        /// <returns>List of statuses returned by Actions</returns>
        public List<string> EndOfTurn()
        {
            List<string> result = new List<string>();
            //Process Actions
            foreach (ShipPart part in Parts.Where(f => !f.IsDestroyed))
            {
                result.Add(part.DoAction(this));
            }

            //Cleanup Weapon Orders
            foreach (ShipOrder weaponOrder in Orders.Where(f => f is ITacticalWeaponOrder))
            {
                if (((Ship)weaponOrder.OrderValues[1]).IsDestroyed)
                    CompletedOrders.Add(weaponOrder);
            }
            //Cleanup MoveToShip Orders
            foreach (ShipOrder moveOrder in Orders.Where(f => f is ITacticalMoveToShipOrder))
            {
                if (((Ship)moveOrder.OrderValues[0]).IsDestroyed)
                    CompletedOrders.Add(moveOrder);
            }

            // clear up completed orders
            if (CompletedOrders.Count > 0)
            {
                foreach (ShipOrder order in CompletedOrders)
                    Orders.Remove(order);
                CompletedOrders.Clear();
            }

            return result.Where(f=>f!=string.Empty).ToList<string>();


        }

        public override string ToString()
        {
            if (this.Name != string.Empty)
                return string.Format("{0}{1} (HP:{2}/{3})",
                    this.Name,
                    this.ClassName,
                    this.HP.Current,
                    this.HP.Max);
            else
                return string.Format("{0}{1} (HP:{2}/{3})",
                    this.ClassName,
                    (this.HullType.Name != string.Empty ? string.Format(" ({0})", this.HullType.Name) : string.Empty),
                    this.HP.Current,
                    this.HP.Max);
        }

        /// <summary>
        /// Used to clone a Ship, for grabbing a copy of a pre-built ship from a list of available definitions (Factory).
        /// </summary>
        /// <returns>The new Ship object</returns>
        public Ship Clone()
        {
            Ship result = new Ship();
            result.ClassName = this.ClassName;
            foreach (ShipPart part in this.Parts)
                result.Parts.Add(part.Clone());
            result.HullType = this.HullType.Clone();
            result.MP.Max = this.MP.Max;
            result.initImage(this.Image.Source,40);
            return result;
        }
        #endregion

        

        #region Serialization
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("HP", HP);
            info.AddValue("MP", MP);
            foreach (ShipPart part in Parts)
                part.Target = null;
            info.AddValue("Equipment", Parts);
            info.AddValue("ClassName", ClassName);
            info.AddValue("HullType", HullType);
            info.AddValue("Orders", Orders);

        }

        /// <summary>
        /// Adds or Updates XML elements for this Ship in supplied XDocument
        /// </summary>
        /// <param name="sourceDoc">XDocument containing "ship" elements for Addition/Update</param>
        public void GetObjectXML(XDocument sourceDoc)
        {
            XElement ship;

            if (sourceDoc.Descendants("ship").Where(f => f.Attribute("className").Value == this.ClassName).Count() > 0)
            {
                // Update Existing
                ship = sourceDoc.Descendants("ship").First(f => f.Attribute("className").Value == this.ClassName);
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
                    new XElement("ship", new XAttribute("className", this.ClassName),
                        new XElement("MaxHP", HP.Max.ToString()),
                        new XElement("MaxMP", MP.Max.ToString()),
                        new XElement("ShipHull",this.HullType.Name),
                        parts);
                sourceDoc.Element("ships").Add(ship);
            }
        }

        void addPartsXML(XElement shipParts)
        {
            List<XElement> parts = new List<XElement>();
            foreach (var shipPart in this.Parts)
                parts.Add(new XElement("shipPart", shipPart.Name));
            shipParts.RemoveAll();
            foreach (var element in parts)
                shipParts.Add(element);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public Ship()
        {
            /* Empty Constructor */
        }
        public Ship(SerializationInfo info, StreamingContext ctxt)
            :base(info,ctxt)
        {
            HP = (StatWithMax)info.GetValue("HP", typeof(StatWithMax));
            MP = (StatWithMax)info.GetValue("MP", typeof(StatWithMax));
            Parts = (List<ShipPart>)info.GetValue("Equipment", typeof(List<ShipPart>));
            Orders = (List<ShipOrder>)info.GetValue("Orders", typeof(List<ShipOrder>));
            ClassName = (string)info.GetValue("ClassName", typeof(string));
            HullType = (ShipHull)info.GetValue("HullType", typeof(ShipHull));
        }
        /// <summary>
        /// Instantiates a Ship based on the XML description, cloning Parts and Hull from 
        /// available objects and assigns ownership to supplied Player
        /// </summary>
        /// <param name="description">XElement containing full description of this Ship</param>
        /// <param name="partsList">List of ShipParts to clone components from</param>
        /// <param name="hullsList">List of ShipHulls to clone hull from</param>
        /// <param name="player">Player to assign ownership to</param>
        public Ship(XElement description, List<EidosPart> partsList, List<ShipHull> hullsList, Player player)
        {
            this.ClassName = description.Attribute("className").Value;
            this.HP.Max = int.Parse(description.Element("MaxHP").Value);
            this.MP.Max = int.Parse(description.Element("MaxMP").Value);
            foreach (XElement SPE in description.Element("shipParts").Elements())
                this.Parts.Add(((ShipPart)partsList.First(f => f.Name == SPE.Value)).Clone());
            if (description.Element("ShipHull") != null)
                if (hullsList.Where(f => f.Name == description.Element("ShipHull").Value).Count() > 0)
                    this.HullType = hullsList.First(f => f.Name == description.Element("ShipHull").Value).Clone();
            this._owner = player;
        }
        #endregion

    }
}
