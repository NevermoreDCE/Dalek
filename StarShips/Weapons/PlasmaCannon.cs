using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;
using StarShips.Randomizer;

namespace StarShips.Weapons
{
    [Serializable]
    public class PlasmaCannon:ShipPart,IWeapon
    {
        int _weaponDamage = 12;
        int _critMultiplier = 3;
        int _reloadTime = 2;
        int _currentReload = 0;

        Ship _target;

        public Ship Target
        {
            get { return _target; }
            set { _target = value; }
        }

        
        public bool IsLoaded
        {
            get {
                if (_currentReload == 0)
                    return true;
                else
                    return false;
            }
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

        public override string ToString()
        {
            return string.Format("{0} (DMG: {1}, {2}x on Crit, {3} Reload)", this.Name, _weaponDamage.ToString(), _critMultiplier.ToString(), _reloadTime.ToString());
        }


        public PlasmaCannon()
        {
            this.Name = "Plasma Cannon";
        }

        public override string Repair(int amount)
        {
            this.IsDestroyed = false;
            return string.Empty;
        }
    }
}
