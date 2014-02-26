using System;
using System.Runtime.Serialization;

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
        #endregion

        #region Public Methods
        public override string ToString()
        {
            return string.Format("{0} ({1}, {2})", Name, EmpireName, IconSet);
        }
        #endregion

        #region Constructors
        public Player(string Name, string EmpireName, string IconSet)
        {
            this._name = Name;
            this._empireName = EmpireName;
            this._iconSet = IconSet;
            this.Ships = new ShipCollection();
            this.IsTurnComplete = false;

            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            System.Windows.Media.Imaging.BitmapImage src = new System.Windows.Media.Imaging.BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(string.Format("Images\\Empires\\{0}\\{0}.png",_iconSet), UriKind.Relative);
            src.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            src.EndInit();
            img.Source = src;
            img.Height = 32;
            img.Width = 32;
            img.Stretch = System.Windows.Media.Stretch.None;
            img.SetValue(System.Windows.Controls.Panel.ZIndexProperty, 10);
            this._icon = img;
        }
        public Player(SerializationInfo info, StreamingContext context)
        {
            this._name = (string)info.GetValue("Name", typeof(string));
            this._empireName = (string)info.GetValue("EmpireName", typeof(string));
            this._iconSet = (string)info.GetValue("IconSet", typeof(string));
            this._icon = (System.Windows.Controls.Image)info.GetValue("Icon", typeof(System.Windows.Controls.Image));
            this.Ships = (ShipCollection)info.GetValue("Ships", typeof(ShipCollection));
            this.IsTurnComplete = (bool)info.GetValue("IsTurnComplete", typeof(bool));
        }
        #endregion

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", this._name);
            info.AddValue("EmpireName", this._empireName);
            info.AddValue("IconSet", this._iconSet);
            info.AddValue("Icon", this._icon);
            info.AddValue("Ships", this.Ships);
            info.AddValue("IsTurnComplete", this.IsTurnComplete);
        }
        #endregion
    }
}
