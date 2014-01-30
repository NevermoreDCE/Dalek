using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace StarShips
{
    public abstract class ShipAction
    {
        #region Parameters
        object[] _actionValues = new object[1];
        public object[] ActionValues { get { return _actionValues; } set { _actionValues = value; } }
        #endregion

        #region Abstract Methods
        public abstract string DoAction(ShipPart target);
        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
        public abstract string ToString();
        #endregion
    }
}
