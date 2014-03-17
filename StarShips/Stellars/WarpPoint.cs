using StarShips.StarSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace StarShips.Stellars
{
    [Serializable]
    public class WarpPoint : Eidos, ISerializable
    {
        #region Private Variables
        StarSystem _linkedSystem;
        WarpPoint _linkedWarpPoint;
        #endregion
        #region Public Properties
        public StarSystem LinkedSystem
        {
            get { return _linkedSystem; }
            set { _linkedSystem = value; }
        }
        public WarpPoint LinkedWarpPoint
        {
            get { return _linkedWarpPoint; }
            set { _linkedWarpPoint = value; }
        }
        public override System.Windows.Controls.Image Image
        {
            get
            {
                if (_image == null)
                {
                    System.Windows.Media.Imaging.BitmapImage src = new System.Windows.Media.Imaging.BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri("Images\\WormHole.png", UriKind.Relative);
                    src.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    src.EndInit();
                    initImage(src, 10);
                }
                return _image;
            }
            set { _image = value; }
        }
        #endregion

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
