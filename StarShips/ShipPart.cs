using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;

namespace StarShips
{
    [Serializable]
    public abstract class ShipPart
    {
        public StatWithMax HP = new StatWithMax();
        public string Name = "No Name";
        public bool IsDestroyed = false;
        internal Ship _target;
        public Ship Target { get { return _target; } set { _target = value; } }
        internal List<IShipPartAction> _actions = new List<IShipPartAction>();
        public List<IShipPartAction> Actions { get { return _actions; } set { _actions = value; } }
        public int PointCost = 0;
        
        
        public abstract override string ToString();
        public abstract string Repair(int amount);
        public string DoAction(Ship target)
        {
            if (_target == null)
                _target = target;
            List<string> results = new List<string>();
            foreach (var action in _actions)
                results.Add(action.DoAction(this));
            return string.Join(", ", results);
        }
    }
}
