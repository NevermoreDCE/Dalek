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
        private ArrayList _ships = new ArrayList();
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

        #region Private Variables
        int _internalIndex = 0;
        #endregion

        #region Public Methods
        public Ship GetNextShip()
        {
            Ship result = null;
            //determine if next InternalIndex is not inside Array
            if (_internalIndex + 1 > _ships.Count)
                _internalIndex = 0;    //roll InternalIndex back to 0
            else
                _internalIndex++;
            
            //find next ship without orders
            int countSearched = 0;
            int tempIndex = _internalIndex;
            while (result == null && countSearched < _ships.Count)
            {
                if (((Ship)_ships[tempIndex]).Orders.Count == 0)
                    result = (Ship)_ships[tempIndex];
                else if (tempIndex + 1 > _ships.Count)
                    tempIndex = 0;
                else
                    tempIndex++;

                countSearched++;
            }

            //if no ship without orders, just get next ship
            if (result == null && _ships.Count > 0)
                result = (Ship)_ships[_internalIndex];
            
            return result;
        }
        #endregion

        #region Constructors
        public ShipCollection()
        {
            /* Empty Constructor */
        }
        public ShipCollection(SerializationInfo info, StreamingContext context)
        {
            this._ships = (ArrayList)info.GetValue("Ships", typeof(ArrayList));
            this._internalIndex = (int)info.GetValue("InternalIndex", typeof(ArrayList));
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
        public ArrayList _ships;

        int position = -1;

        public ShipColEnum(ArrayList ships)
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
