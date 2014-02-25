using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Specialized;

namespace StarShips
{
    [Serializable]
    public class ShipCollection:ISerializable, INotifyCollectionChanged, IEnumerable<Ship>
    {
        #region Collection and Indexer
        private List<Ship> _ships = new List<Ship>();
        public Ship this[int index]
        {
            get
            {
                return (Ship)_ships[index];
            }
            set
            {
                if (value is Ship)
                {
                    _ships[index] = value;
                    if (CollectionChanged != null)
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value));
                }
            }
        }
        public int Count
        {
            get { return _ships.Count; }
        }
        public void Add(Ship ship)
        {
            _ships.Add(ship);
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, ship));
        }
        public void Remove(Ship ship)
        {
            _ships.Remove(ship);
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, ship));
        }
        #endregion

        #region Internal Index Counter
        int _internalIndex = 0;
        private int NextIndex()
        {
            if (_internalIndex + 1 >= _ships.Count)
                return 0;
            else
                return _internalIndex + 1;
        }
        private void IncreaseIndex()
        {
            if (_internalIndex + 1 >= _ships.Count)
                _internalIndex = 0;
            else
                _internalIndex++;
        }
        #endregion

        #region Public Methods
        public Ship GetNextShip()
        {
            return GetNextShip(false);
        }
        public Ship GetNextShip(bool includeDestroyed)
        {
            Ship result = null;
            if (!includeDestroyed)
            {
                if (_ships.Any(f => f.IsDestroyed == false))
                {
                    bool indexAdjusted = false;
                    while (!indexAdjusted)
                    {

                        if (_ships[NextIndex()].IsDestroyed)
                        {
                            IncreaseIndex();
                            indexAdjusted = true;
                        }
                    }
                }
                else
                    return _ships[0];
            }
            IncreaseIndex();
            
            //find next ship without orders
            int countSearched = 0;
            int tempIndex = _internalIndex;
            while (result == null && countSearched < _ships.Count)
            {
                if (((Ship)_ships[tempIndex]).Orders.Count == 0 && (includeDestroyed || !((Ship)_ships[tempIndex]).IsDestroyed))
                    result = (Ship)_ships[tempIndex];
                else if (tempIndex + 1 >= _ships.Count)
                    tempIndex = 0;
                else
                    tempIndex++;

                countSearched++;
            }

            //if no ship without orders, just get next ship
            if (result == null && _ships.Count > 0 && (includeDestroyed || !((Ship)_ships[_internalIndex]).IsDestroyed))
                result = (Ship)_ships[_internalIndex];
            
            return result;
        }
        public void ResetIndex()
        {
            _internalIndex = -1;
        }
        #endregion

        #region Constructors
        public ShipCollection()
        {
            /* Empty Constructor */
        }
        public ShipCollection(SerializationInfo info, StreamingContext context)
        {
            this._ships = (List<Ship>)info.GetValue("Ships", typeof(List<Ship>));
            this._internalIndex = (int)info.GetValue("InternalIndex", typeof(int));
        }
        #endregion

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Ships", this._ships);
            info.AddValue("InternalIndex", this._internalIndex);
        }
        #endregion

        #region Events
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion

        #region IEnumerable Methods
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public ShipColEnum GetEnumerator()
        {
            return new ShipColEnum(_ships);
        }

        IEnumerator<Ship> IEnumerable<Ship>.GetEnumerator()
        {
            return (IEnumerator<Ship>)GetEnumerator();
        }
        #endregion
    }

    public class ShipColEnum : IEnumerator<Ship>
    {
        public List<Ship> _ships;

        int position = -1;

        public ShipColEnum(List<Ship> ships)
        {
            _ships = ships;
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
        public Ship Current
        {
            get
            {
                try
                {
                    return (Ship)_ships[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public bool MoveNext()
        {
            position++;
            return (position < _ships.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        public void Dispose()
        {
            _ships = null;
        }
    }
}
