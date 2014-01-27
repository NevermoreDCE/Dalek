using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using StarShips.Interfaces;

namespace StarShips.PartBase
{
    [Serializable]
    public class ActionPart : ShipPart, ISerializable
    {
        #region Private Variables
        string _actionDescription = string.Empty;
        #endregion

        #region Public Properties
        #endregion

        #region Public Methods
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, _actionDescription);
        }

        public override string Repair(int amount)
        {
            this.IsDestroyed = false;
            return string.Empty;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("HP", HP);
            info.AddValue("ActionDescription", _actionDescription);
            info.AddValue("Actions", _actions);
        }
        #endregion

        #region Constructors
        public ActionPart(string Name, int MaxHP, string ActionDescription, List<IShipPartAction> Actions)
        {
            this.Name = Name;
            HP.Max = MaxHP;
            HP.Current = MaxHP;
            _actionDescription = ActionDescription;
            _actions = Actions;
        }

        public ActionPart(SerializationInfo info, StreamingContext ctxt)
        {
            Name = (string)info.GetValue("Name", typeof(string));
            HP = (StatWithMax)info.GetValue("HP", typeof(StatWithMax));
            _actionDescription = (string)info.GetValue("ActionDescription", typeof(string));
            _actions = (List<IShipPartAction>)info.GetValue("Actions", typeof(List<IShipPartAction>));
        }
        #endregion

    }
}
