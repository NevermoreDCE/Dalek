using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using System.Collections;

namespace StarShips.Players
{
    public class PlayerCollection : ISerializable, INotifyCollectionChanged, IEnumerable<Player>
    {
        #region Collection and Indexer
        ArrayList _players = new ArrayList();
        public Player this[int index]
        {
            get
            {
                return (Player)_players[index];
            }
            set
            {
                if (value is Player)
                {
                    _players[index] = value;
                    if (CollectionChanged != null)
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value));
                }
            }
        }
        public int Count
        {
            get
            {
                return _players.Count;
            }
        }
        public void Add(Player player)
        {
            _players.Add(player);
            if(CollectionChanged!=null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, player));
        }
        public void Remove(Player player)
        {
            _players.Remove(player);
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, player));
        }
        public void Clear()
        {
            _players.Clear();
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        #endregion

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Players", _players);
        }
        #endregion

        #region Constructors
        public PlayerCollection()
        {
            /* Empty Constructor */
        }
        public PlayerCollection(SerializationInfo info, StreamingContext context)
        {
            _players = (ArrayList)info.GetValue("Players", typeof(ArrayList));
        }
        #endregion

        #region Events
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion

        #region IEnumerable Methods
        IEnumerator<Player> IEnumerable<Player>.GetEnumerator()
        {
            return (IEnumerator<Player>)GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
        
        public PlayerColEnum GetEnumerator()
        {
            return new PlayerColEnum(_players);
        }
        #endregion

        #region Internal Variables
        int _internalIndex = 0;
        #endregion
        public Player Next()
        {
            if (_internalIndex + 1 >= _players.Count)
                _internalIndex = 0;
            else
                _internalIndex++;
            return (Player)_players[_internalIndex];
        }
    }

    public class PlayerColEnum : IEnumerator<Player>
    {
        public ArrayList _players;
        int position = -1;

        public PlayerColEnum(ArrayList Players)
        {
            _players = Players;
        }

        public Player Current
        {
            get
            {
                try
                {
                    return (Player)_players[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public void Dispose()
        {
            _players = null;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public bool MoveNext()
        {
            position++;
            return (position < _players.Count);
        }

        public void Reset()
        {
            position = -1;
        }
    }

}
