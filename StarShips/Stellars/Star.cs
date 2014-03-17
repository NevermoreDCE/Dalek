using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace StarShips.Stellars
{
    public class Star : Eidos, ISerializable
    {
        #region Private Variables
        string _imageURL;
        #endregion
        #region Public Properties
        public override System.Windows.Controls.Image Image
        {
            get
            {
                if (_image == null)
                {
                    System.Windows.Media.Imaging.BitmapImage src = new System.Windows.Media.Imaging.BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri(_imageURL, UriKind.Relative);
                    src.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    src.EndInit();
                    _image = new System.Windows.Controls.Image();
                    _image.Source = src;
                    _image.Height = 32;
                    _image.Width = 32;
                    _image.Stretch = System.Windows.Media.Stretch.None;
                    _image.SetValue(System.Windows.Controls.Panel.ZIndexProperty, 10);
                }
                return _image;
            }
            set { _image = value; }
        }
        #endregion

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info,context);
            info.AddValue("ImageURL", _imageURL);
        }
        #endregion

        #region Constructors
        public Star()
        {
            /* Empty Constructor */
        }
        public Star(SerializationInfo info, StreamingContext context)
            : base(info,context)
        {
            _imageURL = (string)info.GetValue("ImageURL", typeof(string));
        }
        public Star(string ImageURL)
        {
            _imageURL = ImageURL;
        }
        #endregion
    }
}
