using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;
using System.Security.Cryptography;
using StarShips.Randomizer;

namespace StarShips.Weapons
{
    [Serializable]
    public class LaserBeam: ShipPart,IWeapon
    {
        int _weaponDamage = 5;
        int _critMultiplier = 2;
        int _reloadTime = 0;
        //int _weaponRange = 1000;
        
        
        bool _isLoaded = true;

        public bool IsLoaded
        {
            get { return _isLoaded; }

        }
        
        public string Fire()
        {
            string result = string.Empty;
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
            return 0;
        }

        public override string ToString()
        {
            return string.Format("{0} (DMG: {1}, {2}x on Crit, {3} Reload)", this.Name, _weaponDamage.ToString(), _critMultiplier.ToString(), _reloadTime.ToString());
        }

        public LaserBeam()
        {
            this.Name = "Laser Beam";
        }

        public override string Repair(int amount)
        {
            this.IsDestroyed = false;
            return string.Empty;
        }
    }
}
