using StarShips.Players;
using StarShips.StarSystems;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Media;
using System.Xml.Linq;

namespace StarShips
{
    /// <summary>
    /// Eidos from Greek εἶδος, meaning Form or Shape, origin of -oid suffix (such as Asteroid or Meteoroid)
    /// </summary>
    public abstract class Eidos : ISerializable
    {
        #region Private Variables
        protected string _name;
        protected System.Windows.Controls.Image _image;
        protected bool _isDestroyed = false;
        protected Player _owner;
        protected List<ShipPart> _parts = new List<ShipPart>();
        protected Point _tacticalPosition = new Point(-1, -1);
        private Point _strategicPosition = new Point(-1, -1);
        private StarSystem _strategicSystem;
        #endregion


        #region Public Properties
        public string Name { get { return _name; } set { _name = value; } }
        public Point TacticalPosition { get { return _tacticalPosition; } set { _tacticalPosition = value; } }
        public bool IsDestroyed { get { return _isDestroyed; } }
        public Player Owner { get { return _owner; } set { _owner = value; } }
        public List<ShipPart> Parts { get { return _parts; } set { _parts = value; } }
        public Point StrategicPosition { get { return _strategicPosition; } set { _strategicPosition = value; } }
        public StarSystem StrategicSystem { get { return _strategicSystem; } set { _strategicSystem = value; } }
        #endregion

        #region Abstract Properties
        public abstract System.Windows.Controls.Image Image { get; set; }
        #endregion

        #region Public Methods
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name); 
            info.AddValue("Owner", _owner);
            info.AddValue("IsDestroyed", IsDestroyed);
            info.AddValue("TacticalPosition", TacticalPosition);
            info.AddValue("StrategicPosition", StrategicPosition);
            info.AddValue("StrategicSystem", StrategicSystem);
        }
        #endregion
        #region Protected Methods
        protected void initImage(ImageSource source, int zIndex)
        {
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.Source = source;
            img.Height = 32;
            img.Width = 32;
            img.Stretch = System.Windows.Media.Stretch.None;
            img.SetValue(System.Windows.Controls.Panel.ZIndexProperty, zIndex);
            this._image = img;
        }
        #endregion
        #region Abstract Constructors
        protected Eidos()
        { 
            /* Empty Constructor */
        }
        protected Eidos(SerializationInfo info, StreamingContext ctxt)
        {
            _owner = (Player)info.GetValue("Owner", typeof(Player));
            _name = (string)info.GetValue("Name",typeof(string));
            _isDestroyed = (bool)info.GetValue("IsDestroyed", typeof(bool));
            _tacticalPosition = (Point)info.GetValue("Position", typeof(Point));
        }
        #endregion
    }
}
