using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;
using StarShips.Randomizer;
using System.Runtime.Serialization;
using System.Xml.Linq;
using StarShips.Utility;

namespace StarShips.Parts
{
    [Serializable]
    public class WeaponPart : ShipPart, ISerializable
    {
        
        #region Private Variables
        int _weaponDamage;
        int _critMultiplier;
        int _reloadTime;
        int _currentReload = 0;
        string _damageType = string.Empty;
        string _firingType = string.Empty;
        double _weaponRange = 0d;
        #endregion

        #region Public Properties

        public bool IsLoaded
        {
            get
            {
                if (_currentReload == 0)
                    return true;
                else
                    return false;
            }
        }

        public int WeaponDamage { get { return _weaponDamage; } }
        public int CritMultiplier { get { return _critMultiplier; } }
        public int ReloadTime { get { return _reloadTime; } }
        public string DamageType { get { return _damageType; } }
        public string FiringType { get { return _firingType; } }
        public double Range { get { return _weaponRange; } }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return string.Format("{0} ({1}{2}DMG: {3}{4}, {5}x on Crit, {6} Reload)",
                this.Name,
                (_firingType != string.Empty ? string.Format("{0} ", _firingType) : string.Empty),
                (_weaponRange > 0d ? string.Format("Rng: {0} ", _weaponRange.ToString()) : string.Empty),
                _weaponDamage.ToString(),
                (_damageType != string.Empty ? string.Format(" - ({0})", _damageType) : string.Empty),
                _critMultiplier.ToString(),
                (_currentReload > 0 ? string.Format("{0}/{1}", _currentReload, _reloadTime) : _reloadTime.ToString())
                );
        }

        /// <summary>
        /// Fires the weapon at its existing target with a fixed 90% hit 5% crit chance
        /// </summary>
        /// <returns>Status result</returns>
        public List<string> Fire()
        {
            return Fire(11, 96); // 10% miss, 5% crit
        }
        /// <summary>
        /// Fires the weapon at its existing target with specific hit/crit chances
        /// </summary>
        /// <param name="hitAbove">Minimum number needed (out of 100) to hit the target</param>
        /// <param name="critAbove">Minimum number needed (out of 100) to critically-hit the target</param>
        /// <returns>Status result</returns>
        public List<string> Fire(int hitAbove, int critAbove)
        {
            List<string> result = new List<string>();
            _currentReload = _reloadTime;
            using (RNG rng = new RNG())
            {
                int hitNum = rng.d100();

                if (hitNum >= critAbove) 
                {
                    result.Add(string.Format("{0} CRITS {1} for {2}",
                        this.Name,
                        Target.ClassName,
                        this.WeaponDamage
                        ));
                    result = result.Concat(Target.HitFor(_weaponDamage * _critMultiplier)).ToList<string>();
                }
                else if (hitNum >= hitAbove)
                {
                    result.Add(string.Format("{0} hits {1} for {2}",
                        this.Name,
                        Target.ClassName,
                        this.WeaponDamage
                        ));
                    result = result.Concat(Target.HitFor(_weaponDamage)).ToList<string>();
                }
                else
                    result.Add(string.Format("{0} Missed!", this.Name));
            }
            
            return result;
        }
        
        /// <summary>
        /// Reduces the reload counter for the weapon
        /// </summary>
        /// <returns>Number of turns remaining until the weapon is reloaded</returns>
        public int Reload()
        {
            if (_currentReload > 0)
                return _currentReload--;
            else
                return 0;
        }

        #endregion

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("HP", HP);
            info.AddValue("Mass", Mass);
            info.AddValue("Damage", _weaponDamage);
            info.AddValue("CritMultiplier", _critMultiplier);
            info.AddValue("DamageType", _damageType);
            info.AddValue("FiringType", _firingType);
            info.AddValue("ReloadTime", _reloadTime);
            info.AddValue("CurrentReload", _currentReload);
            info.AddValue("Actions", _actions);
            info.AddValue("Parent", _parent);
            info.AddValue("WeaponRange", _weaponRange);
        }

        public override void GetObjectXML(XDocument sourceDoc)
        {
            XElement weap;

            if (sourceDoc.Descendants("weaponPart").Where(f => f.Attribute("name").Value == this.Name).Count() > 0)
            {
                // Update Existing
                weap = sourceDoc.Descendants("weaponPart").First(f => f.Attribute("name").Value == this.Name);
                weap.Element("MaxHP").Value = this.HP.Max.ToString();
                if (weap.Element("Mass") != null)
                    weap.Element("Mass").Value = this._mass.ToString();
                else
                    weap.Add(new XElement("Mass", this._mass.ToString()));
                weap.Element("WeaponDamage").Value = this._weaponDamage.ToString();
                weap.Element("CritMultiplier").Value = this._critMultiplier.ToString();
                if (weap.Element("DamageType") != null)
                    weap.Element("DamageType").Value = this._damageType;
                else
                    weap.Add(new XElement("DamageType", this._damageType));
                if (weap.Element("FiringType") != null)
                    weap.Element("FiringType").Value = this._firingType;
                else
                    weap.Add(new XElement("FiringType", this._firingType));
                if (weap.Element("WeaponRange") != null)
                    weap.Element("WeaponRange").Value = this._weaponRange.ToString();
                else
                    weap.Add(new XElement("WeaponRange", this._weaponRange.ToString()));
                weap.Element("ReloadTime").Value = this._reloadTime.ToString();
                
                
                addActions(weap.Element("Actions"));
            }
            else
            {
                // Create New
                XElement actions = new XElement("Actions");
                addActions(actions);
                weap =
                    new XElement("weaponPart", new XAttribute("name", this.Name),
                        new XElement("MaxHP", this.HP.Max.ToString()),
                        new XElement("Mass",this.Mass.ToString()),
                        new XElement("WeaponDamage", this._weaponDamage.ToString()),
                        new XElement("CritMultiplier", this._critMultiplier.ToString()),
                        new XElement("DamageType",this._damageType),
                        new XElement("FiringType",this._firingType),
                        new XElement("ReloadTime", this._reloadTime.ToString()),
                        new XElement("WeaponRange",this._weaponRange.ToString()),
                        actions);
                sourceDoc.Descendants("weaponParts").First().Add(weap);
            }
        }

        #endregion

        #region Constructors
        private void initWeaponPart(Ship Parent, string Name, int MaxHP, double Mass, int Damage,double Range, string DamageType, string FiringType, int CritMultiplier, int ReloadTime, List<ShipAction> Actions)
        {
            this._parent = Parent;
            this.Name = Name;
            HP.Max = MaxHP;
            HP.Current = MaxHP;
            _mass = Mass;
            _weaponDamage = Damage;
            _critMultiplier = CritMultiplier;
            _damageType = DamageType;
            _firingType = FiringType;
            _reloadTime = ReloadTime;
            _actions = Actions;
            _weaponRange = Range;
        }
        public WeaponPart(Ship Parent, string Name, int MaxHP, double Mass, int Damage, double Range, string DamageType, string FiringType, int CritMultiplier, int ReloadTime, List<ShipAction> Actions)
        {
            initWeaponPart(Parent, Name, MaxHP, Mass, Damage, Range, DamageType, FiringType, CritMultiplier, ReloadTime, Actions);
        }
        public WeaponPart(SerializationInfo info, StreamingContext ctxt)
        {
            this._parent = (Ship)info.GetValue("Parent", typeof(Ship));
            this.Name = (string)info.GetValue("Name", typeof(string));
            HP = (StatWithMax)info.GetValue("HP", typeof(StatWithMax));
            _mass = (double)info.GetValue("Mass", typeof(double));
            _weaponDamage = (int)info.GetValue("Damage", typeof(int));
            _critMultiplier = (int)info.GetValue("CritMultiplier", typeof(int));
            _damageType = (string)info.GetValue("DamageType", typeof(string));
            _firingType = (string)info.GetValue("FiringType", typeof(string));
            _reloadTime = (int)info.GetValue("ReloadTime", typeof(int));
            _currentReload = (int)info.GetValue("CurrentReload", typeof(int));
            _actions = (List<ShipAction>)info.GetValue("Actions", typeof(List<ShipAction>));
            _weaponRange = (double)info.GetValue("WeaponRange", typeof(double));
        }
        public WeaponPart(XElement description, Ship parent)
        {
            this._parent = parent;
            this.Name = description.Attribute("name").Value;
            this.HP.Max = int.Parse(description.Element("MaxHP").Value);
            if (description.Element("Mass") != null)
                this._mass = double.Parse(description.Element("Mass").Value);
            this._weaponDamage = int.Parse(description.Element("WeaponDamage").Value);
            this._critMultiplier = int.Parse(description.Element("CritMultiplier").Value);
            if (description.Element("DamageType") != null)
                this._damageType = description.Element("DamageType").Value;
            if (description.Element("FiringType") != null)
                this._firingType = description.Element("FiringType").Value;
            this._reloadTime = int.Parse(description.Element("ReloadTime").Value);
            if (description.Element("WeaponRange") != null)
                this._weaponRange = double.Parse(description.Element("WeaponRange").Value);
            loadActions(description.Element("Actions"));
        }
        #endregion


        
    }
}
