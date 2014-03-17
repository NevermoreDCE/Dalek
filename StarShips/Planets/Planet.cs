using System;
using System.Runtime.Serialization;

namespace StarShips.Planets
{
    [Serializable]
    public class Planet : Eidos, ISerializable
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
                    initImage(src,10);
                }
                return _image;
            }
            set { _image = value; }
        }
        #endregion

        #region Serialization
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ImageURL", _imageURL);
        }
        #endregion

        #region Constructors
        public Planet() : base()
        {
            /* Empty Constructor */
        }
        public Planet(string ImageURL)
        {
            _imageURL = ImageURL;
        }
        public Planet(SerializationInfo info, StreamingContext context)
            : base(info,context)
        {
            this._imageURL = (string)info.GetValue("ImageURL", typeof(string));
        }
        #endregion
    }
}
