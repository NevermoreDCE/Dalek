using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;
using System.Xml.Linq;

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
        public abstract void GetObjectXML(XDocument sourceDoc);


        public string DoAction(Ship target)
        {
            if (_target == null)
                _target = target;
            List<string> results = new List<string>();
            foreach (var action in _actions)
                results.Add(action.DoAction(this));
            return string.Join(", ", results);
        }

        internal void addActions(XElement ActionsElement)
        {
            List<XElement> actions = new List<XElement>();
            XElement action;
            XElement actionIndex;
            foreach (var act in this.Actions)
            {
                action = new XElement("action", new XAttribute("type", act.GetType().ToString()));
                for (int i = 0; i < act.ActionValues.Length; i++)
                {
                    actionIndex = new XElement("Value", new XAttribute("index", i));
                    actionIndex.Value = act.ActionValues[i].ToString();
                    action.Add(actionIndex);
                }
                actions.Add(action);
            }

            ActionsElement.RemoveAll();
            foreach (var element in actions)
                ActionsElement.Add(element);
        }

        internal void loadActions(XElement ActionsElement)
        {
            List<XElement> actionValues;
            IShipPartAction newAction;
            foreach (var action in ActionsElement.Elements())
            {
                actionValues = new List<XElement>();
                foreach(var val in action.Elements("Value"))
                    actionValues.Add(val);

                Type newActionType = Type.GetType(action.Attribute("type").Value);
                if(actionValues.Count>0)
                {
                    int[] actVals = new int[actionValues.Count];
                    foreach(var val in actionValues)
                        actVals[int.Parse(val.Attribute("index").Value)]=int.Parse(val.Value);
                    newAction = (IShipPartAction)Activator.CreateInstance(newActionType, actVals);
                }
                    else
                {
                    newAction = (IShipPartAction)Activator.CreateInstance(newActionType);
                }
                this._actions.Add(newAction);
            }
        }
    }
}
