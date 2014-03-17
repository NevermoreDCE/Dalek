using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace StarShips.StarSystems
{
    [Serializable]
    public class StarSystemCollection : ISerializable, IEnumerable<StarSystem>
    {
        #region Indexer
        private List<StarSystem> _starSystems = new List<StarSystem>();

        public StarSystem this[int index]
        {
            get
            {
                return (StarSystem)_starSystems[index];
            }
            set
            {
                if (value is StarSystem)
                {
                    _starSystems[index] = value;
                }
            }
        }

        public void Add(StarSystem system)
        {
            _starSystems.Add(system);
        }
        #endregion

        #region Public Methods
        public List<Tuple<StarSystem,StarSystem>> GetConnections()
        {
            List<Tuple<StarSystem, StarSystem>> connections = new List<Tuple<StarSystem, StarSystem>>();
            return connections;
        }
        #endregion

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public StarSystemColEnum GetEnumerator()
        {
            return new StarSystemColEnum(_starSystems);
        }

        IEnumerator<StarSystem> IEnumerable<StarSystem>.GetEnumerator()
        {
            return (IEnumerator<StarSystem>)GetEnumerator();
        }
        #endregion
    }

    public class StarSystemColEnum : IEnumerator<StarSystem>
    {
        public List<StarSystem> _starSystems;

        int position = -1;

        public StarSystemColEnum(List<StarSystem> starSystems)
        {
            _starSystems = starSystems;
        }
        public StarSystemColEnum()
        {

        }
        object IEnumerator.Current
        {
            get { return Current; }
        }
        public StarSystem Current
        {
            get
            {
                try
                {
                    return (StarSystem)_starSystems[position];
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
            return (position < _starSystems.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        public void Dispose()
        {
            _starSystems = null;
        }
    }
}
