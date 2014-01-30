using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;
using StarShips.Randomizer;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace StarShips.PartBase
{
    [Serializable]
    public class WeaponPart : ShipPart, IWeapon, ISerializable
    {
        #region Private Variables
        int _weaponDamage;
        int _critMultiplier;
        int _reloadTime;
        int _currentReload = 0;
        string _damageType;
        string _firingType;
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

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return string.Format("{0} ({1}DMG: {2}{3}, {4}x on Crit, {5} Reload)",
                this.Name,
                (_firingType != string.Empty ? string.Format("{0} ", _firingType) : string.Empty),
                _weaponDamage.ToString(),
                (_damageType != string.Empty ? string.Format(" - ({0})", _damageType) : string.Empty),
                _critMultiplier.ToString(),
                _reloadTime.ToString());
        }

        public override string Repair(int amount)
        {
            this.IsDestroyed = false;
            return string.Empty;
        }

        public string Fire()
        {
            return Fire(11, 96); // 10% miss, 5% crit
        }

        public string Fire(int hitAbove, int critAbove)
        {
            string result = string.Empty;
            _currentReload = _reloadTime;
            using (RNG rng = new RNG())
            {
                int hitNum = rng.d100();

                if (hitNum >= critAbove) 
                    result = "Crit! " + string.Join(", ", Target.HitFor(_weaponDamage * _critMultiplier));
                else if (hitNum >= hitAbove) 
                    result = string.Join(", ", Target.HitFor(_weaponDamage));
                else 
                    result = "Missed!";
            }
            return result;
        }

        public int Reload()
        {
            return _currentReload--;
        }

        #endregion

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("HP", HP);
            info.AddValue("Damage", _weaponDamage);
            info.AddValue("CritMultiplier", _critMultiplier);
            info.AddValue("DamageType", _damageType);
            info.AddValue("FiringType", _firingType);
            info.AddValue("ReloadTime", _reloadTime);
            info.AddValue("CurrentReload", _currentReload);
            info.AddValue("Actions", _actions);
        }

        public override void GetObjectXML(XDocument sourceDoc)
        {
            XElement weap;

            if (sourceDoc.Descendants("weaponPart").Where(f => f.Attribute("name").Value == this.Name).Count() > 0)
            {
                // Update Existing
                weap = sourceDoc.Descendants("weaponPart").First(f => f.Attribute("name").Value == this.Name);
                weap.Element("MaxHP").Value = this.HP.Max.ToString();
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
                        new XElement("WeaponDamage", this._weaponDamage.ToString()),
                        new XElement("CritMultiplier", this._critMultiplier.ToString()),
                        new XElement("DamageType",this._damageType),
                        new XElement("FiringType",this._firingType),
                        new XElement("ReloadTime", this._reloadTime.ToString()),
                        actions);
                sourceDoc.Descendants("weaponParts").First().Add(weap);
            }
        }

        #endregion

        #region Constructors
        private void initWeaponPart(string Name, int MaxHP, int Damage, string DamageType, string FiringType, int CritMultiplier, int ReloadTime, List<ShipAction> Actions)
        {
            this.Name = Name;
            HP.Max = MaxHP;
            HP.Current = MaxHP;
            _weaponDamage = Damage;
            _critMultiplier = CritMultiplier;
            _damageType = DamageType;
            _firingType = FiringType;
            _reloadTime = ReloadTime;
            _actions = Actions;
        }

        public WeaponPart(string Name, int MaxHP, int Damage, int CritMultiplier, int ReloadTime, List<ShipAction> Actions)
        {
            initWeaponPart(Name, MaxHP, Damage, string.Empty,string.Empty, CritMultiplier, ReloadTime, Actions);
        }
        public WeaponPart(string Name, int MaxHP, int Damage, string DamageType, int CritMultiplier, int ReloadTime, List<ShipAction> Actions)
        {
            initWeaponPart(Name, MaxHP, Damage, DamageType,string.Empty, CritMultiplier, ReloadTime, Actions);
        }
        public WeaponPart(string Name, int MaxHP, int Damage, string DamageType, string FiringType, int CritMultiplier, int ReloadTime, List<ShipAction> Actions)
        {
            initWeaponPart(Name, MaxHP, Damage, DamageType, FiringType, CritMultiplier, ReloadTime, Actions);
        }
        public WeaponPart(SerializationInfo info, StreamingContext ctxt)
        {
            this.Name = (string)info.GetValue("Name", typeof(string));
            HP = (StatWithMax)info.GetValue("HP", typeof(StatWithMax));
            _weaponDamage = (int)info.GetValue("Damage", typeof(int));
            _critMultiplier = (int)info.GetValue("CritMultiplier", typeof(int));
            _damageType = (string)info.GetValue("DamageType", typeof(string));
            _firingType = (string)info.GetValue("FiringType", typeof(string));
            _reloadTime = (int)info.GetValue("ReloadTime", typeof(int));
            _currentReload = (int)info.GetValue("CurrentReload", typeof(int));
            _actions = (List<ShipAction>)info.GetValue("Actions", typeof(List<ShipAction>));
        }
        public WeaponPart(XElement description)
        {
            this.Name = description.Attribute("name").Value;
            this.HP.Max = int.Parse(description.Element("MaxHP").Value);
            this._weaponDamage = int.Parse(description.Element("WeaponDamage").Value);
            this._critMultiplier = int.Parse(description.Element("CritMultiplier").Value);
            if (description.Element("DamageType") != null)
                this._damageType = description.Element("DamageType").Value;
            if (description.Element("FiringType") != null)
                this._firingType = description.Element("FiringType").Value;
            this._reloadTime = int.Parse(description.Element("ReloadTime").Value);
            loadActions(description.Element("Actions"));
        }
        #endregion


        
    }
}
