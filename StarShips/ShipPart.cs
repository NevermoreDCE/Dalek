﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarShips.Interfaces;
using System.Xml.Linq;
using StarShips.Parts;
using StarShips.Utility;

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
        internal Ship _parent;
        public Ship Parent { get { return _parent; } }
        internal List<ShipAction> _actions = new List<ShipAction>();
        public List<ShipAction> Actions { get { return _actions; } set { _actions = value; } }
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
            ShipAction newAction;
            foreach (var action in ActionsElement.Elements())
            {
                actionValues = new List<XElement>();
                foreach(var val in action.Elements("Value"))
                    actionValues.Add(val);

                Type newActionType = Type.GetType(action.Attribute("type").Value);
                if(actionValues.Count>0)
                {
                    object[] actVals = new object[actionValues.Count];
                    foreach(var val in actionValues)
                        actVals[int.Parse(val.Attribute("index").Value)]=int.Parse(val.Value);
                    newAction = (ShipAction)Activator.CreateInstance(newActionType, actVals);
                }
                    else
                {
                    newAction = (ShipAction)Activator.CreateInstance(newActionType);
                }
                this._actions.Add(newAction);
            }
        }

        public ShipPart Clone()
        {
            ShipPart result;
            if (this is WeaponPart)
            {
                WeaponPart source = (WeaponPart)this;
                List<ShipAction> newActions = new List<ShipAction>();
                foreach (ShipAction oldAct in this.Actions)
                {
                    object[] oldValues = new object[oldAct.ActionValues.Length];
                    for (int i = 0; i < oldValues.Length; i++)
                    {
                        oldValues[i] = oldAct.ActionValues[i];
                    }   
                    Type t = oldAct.GetType();
                    ShipAction newAct = (ShipAction)Activator.CreateInstance(t,oldValues);
                    newActions.Add(newAct);
                }
                result = new WeaponPart(source.Parent, source.Name, source.HP.Max, source.WeaponDamage,source.DamageType,source.FiringType, source.CritMultiplier, source.ReloadTime, newActions);
            }
            else if (this is DefensePart)
            {
                DefensePart source = (DefensePart)this;
                List<ShipAction> newActions = new List<ShipAction>();
                foreach (ShipAction oldAct in this.Actions)
                {
                    object[] oldValues = new object[oldAct.ActionValues.Length];
                    for (int i = 0; i < oldValues.Length; i++)
                    {
                        oldValues[i] = oldAct.ActionValues[i];
                    }   
                    Type t = oldAct.GetType();
                    ShipAction newAct = (ShipAction)Activator.CreateInstance(t,oldValues);
                    newActions.Add(newAct);
                }
                result = new DefensePart(source.Parent, source.Name, source.HP.Max, source.DR, source.DownAdjective, source.PenetrateVerb, newActions);
            }
            else
            {
                ActionPart source = (ActionPart)this;
                List<ShipAction> newActions = new List<ShipAction>();
                foreach (ShipAction oldAct in this.Actions)
                {
                    object[] oldValues = new object[oldAct.ActionValues.Length];
                    for (int i = 0; i < oldValues.Length; i++)
                    {
                        oldValues[i] = oldAct.ActionValues[i];
                    }   
                    Type t = oldAct.GetType();
                    ShipAction newAct = (ShipAction)Activator.CreateInstance(t,oldValues);
                    newActions.Add(newAct);
                }
                result = new ActionPart(source.Parent, source.Name, source.HP.Max, source.Description, newActions);
            }
            return result;
        }

        public static List<ShipPart> GetShipPartList(XDocument sourceDoc, Ship parent)
        {
            List<ShipPart> ShipPartList = new List<ShipPart>();
            XElement weaponParts = sourceDoc.Element("shipParts").Element("weaponParts");
            foreach (XElement weaponPart in weaponParts.Elements())
                ShipPartList.Add(new WeaponPart(weaponPart, parent));
            XElement defenseParts = sourceDoc.Element("shipParts").Element("defenseParts");
            foreach (XElement defensePart in defenseParts.Elements())
                ShipPartList.Add(new DefensePart(defensePart, parent));
            XElement actionParts = sourceDoc.Element("shipParts").Element("actionParts");
            foreach (XElement actionPart in actionParts.Elements())
                ShipPartList.Add(new ActionPart(actionPart, parent));
            return ShipPartList;
        }
    }
}
