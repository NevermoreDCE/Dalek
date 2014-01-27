using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;
using StarShips.Randomizer;
using System.Runtime.Serialization;

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

        #endregion

        #region Private Methods
        #endregion

        #region Public Methods

        public override string ToString()
        {
            return string.Format("{0} (DMG: {1}, {2}x on Crit, {3} Reload)", this.Name, _weaponDamage.ToString(), _critMultiplier.ToString(), _reloadTime.ToString());
        }

        public override string Repair(int amount)
        {
            this.IsDestroyed = false;
            return string.Empty;
        }

        public string Fire()
        {
            string result = string.Empty;
            _currentReload = _reloadTime;
            using (RNG rng = new RNG())
            {
                int hitNum = rng.d100();

                if (hitNum >= 96) // 96-100 is crit
                    result = "Crit! " + string.Join(", ", Target.HitFor(_weaponDamage * _critMultiplier));
                else if (hitNum >= 11) // 11-95 is hit
                    result = string.Join(", ", Target.HitFor(_weaponDamage));
                else // 0-10 is miss
                    result = "Missed!";
            }
            return result;
        }

        public int Reload()
        {
            return _currentReload--;
        }

        #endregion

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("HP", HP);
            info.AddValue("Damage", _weaponDamage);
            info.AddValue("CritMultiplier", _critMultiplier);
            info.AddValue("ReloadTime", _reloadTime);
            info.AddValue("CurrentReload", _currentReload);
            info.AddValue("Actions", _actions);
        }

        #region Constructors
        public WeaponPart(string Name, int MaxHP, int Damage, int CritMultiplier, int ReloadTime, List<IShipPartAction> Actions)
        {
            this.Name = Name;
            HP.Max = MaxHP;
            HP.Current = MaxHP;
            _weaponDamage = Damage;
            _critMultiplier = CritMultiplier;
            _reloadTime = ReloadTime;
            _actions = Actions;
        }
        public WeaponPart(SerializationInfo info, StreamingContext ctxt)
        {
            this.Name = (string)info.GetValue("Name", typeof(string));
            HP = (StatWithMax)info.GetValue("HP", typeof(StatWithMax));
            _weaponDamage = (int)info.GetValue("Damage", typeof(int));
            _critMultiplier = (int)info.GetValue("CritMultiplier", typeof(int));
            _reloadTime = (int)info.GetValue("ReloadTime", typeof(int));
            _currentReload = (int)info.GetValue("CurrentReload", typeof(int));
            _actions = (List<IShipPartAction>)info.GetValue("Actions", typeof(List<IShipPartAction>));
        }
        #endregion


        
    }
}
