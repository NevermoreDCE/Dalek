using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace StarShips
{
    public class ThemeSettings
    {
        #region Private Variables
        List<string> _firingTypes = new List<string>();
        List<string> _damageTypes = new List<string>();
        #endregion
        #region Public Properties
        public List<string> FiringTypes { get { return _firingTypes; } }
        public List<string> DamageTypes { get { return _damageTypes; } }
        #endregion

        #region Constructors
        public ThemeSettings(XDocument settingDoc)
        {
            XElement firing = settingDoc.Element("themeSettings").Element("firingTypes");
            foreach (XElement ft in firing.Elements())
                this._firingTypes.Add(ft.Value);
            XElement damage = settingDoc.Element("themeSettings").Element("damageTypes");
            foreach (XElement dt in damage.Elements())
                this._damageTypes.Add(dt.Value);
        }
        #endregion

    }
}
