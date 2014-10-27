using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using StarShips.Locations;
using StarShips.Parts;
using StarShips.Orders;
using StarShips.StarSystems;

namespace StarShips.Players
{
    [Serializable]
    public class Player:ISerializable
    {
        #region Private Variables
        string _name;
        string _empireName;
        string _iconSet;
        System.Windows.Controls.Image _icon;
        bool _isAI = false;
        int _aggressiveness = 0;
        StarSystem _homeSystem;
        #endregion

        #region Public Properties
        public string Name { get { return _name; } }
        public string EmpireName { get { return _empireName; } }
        public string IconSet { get { return _iconSet; } }
        public System.Windows.Controls.Image Icon { get { return _icon; } }
        public bool IsAI { get { return _isAI; } set { _isAI = value; } }
        public ShipCollection Ships;
        public bool IsTurnComplete;
        public bool IsDefeated = false;
        public StarSystem HomeSystem { get { return _homeSystem; } set { _homeSystem = value; } }
        #endregion

        #region Public Methods
        public override string ToString()
        {
            return string.Format("{0} ({1}, {2})", Name, EmpireName, IconSet);
        }
        #endregion

        #region Private Methods
        void initIcon()
        {
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            System.Windows.Media.Imaging.BitmapImage src = new System.Windows.Media.Imaging.BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(string.Format("Empires\\{0}\\Images\\{0}.png", _iconSet), UriKind.Relative);
            src.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            src.EndInit();
            img.Source = src;
            img.Height = 32;
            img.Width = 32;
            img.Stretch = System.Windows.Media.Stretch.None;
            img.SetValue(System.Windows.Controls.Panel.ZIndexProperty, 10);
            this._icon = img;
        }

        public void ExecuteAI(Game gameState)
        {
            if (Ships.Any(f => f.Orders.Count() == 0))
            {
                Ship[] targetList = new Ship[gameState.Players.Where(f => f != this && !f.IsDefeated).Max(p => p.Ships.Where(s => !s.IsDestroyed).Count())];

                //build lists of targets+distance (jump closest target)
                int counter =0;
                List<Tuple<Ship, double>> AllEnemyShips = new List<Tuple<Ship, double>>();
                foreach(Ship ship in this.Ships.Where(f=>!f.IsDestroyed && f.Orders.Count()==0))
                {
                    foreach(Player p in gameState.Players.Where(f=>f!=this && !f.IsDefeated))
                        foreach(Ship s in p.Ships.Where(f=>!f.IsDestroyed))
                            AllEnemyShips.Add(new Tuple<Ship,double>(s,LocationCollection.GetTacticalDistance(ship.TacticalPosition,s.TacticalPosition)));
                    counter++;
                }

                // take highest target from all lists
                List<TargetTracker> EachEnemyShip = new List<TargetTracker>();
                List<Ship> EnemyShips = new List<Ship>();
                foreach (Tuple<Ship, double> enemyShip in AllEnemyShips)
                {
                    if (!EnemyShips.Contains(enemyShip.Item1))
                        EnemyShips.Add(enemyShip.Item1);
                }
                foreach (Ship enemyShip in EnemyShips)
                {
                    double sumDistance = AllEnemyShips.Where(f=>f.Item1==enemyShip).Sum(f=>f.Item2);
                    EachEnemyShip.Add(new TargetTracker(enemyShip, sumDistance, 0));
                }
                // find closest friendly ship
                foreach (Ship ship in this.Ships.Where(f => !f.IsDestroyed))
                {
                    Ship targetShip = EachEnemyShip.First(f => f.Range == EachEnemyShip.Min(z => z.Range)).Ship;

                    //add move order (Move to target and Minimum Weapon Range)
                    int range = Convert.ToInt32(ship.Parts.Where(f=>!f.IsDestroyed && f is WeaponPart).Min(f=>((WeaponPart)f).Range));
                    MoveToShipAtRange mtsar = new MoveToShipAtRange(targetShip, range, gameState.CombatLocations);
                    ship.Orders.Add(mtsar);

                    //add attack orders (Attack target with all weapons)
                    foreach (WeaponPart weapon in ship.Parts.Where(f=>!f.IsDestroyed && f is WeaponPart))
                    {
                        FireWeaponAtTarget fwat = new FireWeaponAtTarget(weapon, targetShip);
                        ship.Orders.Add(fwat);
                    }

                    // calculate if this ship is enough to destroy target within Aggressiveness
                    // this Damage Per Turn
                    double totalDPT = 0;
                    foreach (WeaponPart weapon in ship.Parts.Where(f => !f.IsDestroyed && f is WeaponPart))
                        if (weapon.ReloadTime > 0)
                            totalDPT += (weapon.WeaponDamage / weapon.ReloadTime);
                        else
                            totalDPT += weapon.WeaponDamage;
                    // miss chance
                    totalDPT = totalDPT * 0.9d;

                    //aggressiveness
                    double totalDPA = totalDPT * _aggressiveness;

                    // target total health
                    double targetHealth = targetShip.HP.Current;
                    foreach (DefensePart defense in targetShip.Parts.Where(f => !f.IsDestroyed && f is DefensePart))
                    {
                        targetHealth += defense.DR + defense.HP.Current;
                    }
                    TargetTracker target = EachEnemyShip.First(f=>f.Ship==targetShip);

                    if (target.TotalDPA + totalDPA >= targetHealth)
                        EachEnemyShip.Remove(EachEnemyShip.First(f => f.Ship == targetShip));
                    else
                        target.TotalDPA += totalDPA;
                }
            }
        }

        #endregion

        #region Constructors
        public Player(string Name, string EmpireName, string IconSet, bool IsAI, int Aggressiveness)
        {
            this._name = Name;
            this._empireName = EmpireName;
            this._iconSet = IconSet;
            this._isAI = IsAI;
            this._aggressiveness = Aggressiveness;
            this.Ships = new ShipCollection();
            this.IsTurnComplete = false;

            initIcon();
        }
        public Player(SerializationInfo info, StreamingContext context)
        {
            this._name = (string)info.GetValue("Name", typeof(string));
            this._empireName = (string)info.GetValue("EmpireName", typeof(string));
            this._iconSet = (string)info.GetValue("IconSet", typeof(string));
            this.Ships = (ShipCollection)info.GetValue("Ships", typeof(ShipCollection));
            this.IsTurnComplete = (bool)info.GetValue("IsTurnComplete", typeof(bool));
            initIcon();
        }
        #endregion

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", this._name);
            info.AddValue("EmpireName", this._empireName);
            info.AddValue("IconSet", this._iconSet);
            info.AddValue("Ships", this.Ships);
            info.AddValue("IsTurnComplete", this.IsTurnComplete);
        }
        #endregion
    }

    class TargetTracker
    {
        public Ship Ship;
        public double Range;
        public double TotalDPA;
        public TargetTracker()
        {

        }
        public TargetTracker(Ship ship, double range, double totalDPA)
        {
            this.Ship = ship;
            this.Range = range;
            this.TotalDPA = totalDPA;
        }
    }
}

