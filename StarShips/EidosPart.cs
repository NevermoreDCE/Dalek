using StarShips.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

namespace StarShips
{
    [Serializable]
    public abstract class EidosPart
    {
        #region Private Variables
        protected StatWithMax _hp = new StatWithMax();
        protected string _name = "No Name";
        protected bool _isDestroyed = false;
        protected Eidos _target;
        protected Eidos _parent;
        protected List<EidosAction> _actions = new List<EidosAction>();
        #endregion

        #region Public Properties
        public StatWithMax HP { get { return _hp; } set { _hp = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public bool IsDestroyed { get { return _isDestroyed; } set { _isDestroyed = value; } }
        public Eidos Target { get { return _target; } set { _target = value; } }
        public Eidos Parent { get { return _parent; } }
        public List<EidosAction> Actions { get { return _actions; } set { _actions = value; } }
        #endregion

        #region Abstract Methods
        public abstract override string ToString();
        public abstract void GetObjectXML(XDocument sourceDoc);
        public abstract string DoAction(Eidos target);
        #endregion

        #region Public Methods
        /// <summary>
        /// Repair the part, removing Destroyed status and adding HP (up to max)
        /// </summary>
        /// <param name="amount">Number of HP to repair</param>
        /// <returns>Status result</returns>
        public string Repair(int amount)
        {
            string result = string.Empty;

            if (this.IsDestroyed)
            {
                result = string.Format("Repaired {0}", this.Name);
                this.IsDestroyed = false;
            }

            if (this.HP.Current < this.HP.Max && amount > 0)
            {
                int amountRepaired = this.HP.Add(amount);
                result = string.Format("Repaired {0} for {1}", this.Name, amountRepaired);
            }

            return result;
        }
        #endregion
    }
}
