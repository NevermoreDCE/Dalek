using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace StarShips.Utility
{
    [Serializable]
    public class StatWithMax : ISerializable
    {
        int _max = 0;
        int _current = 0;
        public int Max { get { return _max; } set { _max = value; _current = value; } }
        public int Current { get { return _current; } set { _current = value; } }
        public override string ToString()
        {
            return string.Format("{0}/{1}", Current.ToString("D" + Max.ToString().Length.ToString()), Max.ToString());
        }
        public int Add(int amount)
        {
            int result;
            if (amount == int.MaxValue)
            {
                result = Max;
                Current = Max;
            }
            else
            {
                result = Math.Min(Max, Current + amount) - Current;
                Current = Math.Min(Max, Current + amount);
            }
            return result;
        }
        public int Reduce(int amount)
        {
            int result = Math.Max(0, Current - amount) + Current;
            Current = Math.Max(0, Current - amount);
            return result;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Max", Max);
            info.AddValue("Current", Current);
        }

        public StatWithMax(SerializationInfo info, StreamingContext ctxt)
        {
            Max = (int)info.GetValue("Max", typeof(int));
            Current = (int)info.GetValue("Current", typeof(int));
        }

        public StatWithMax()
        {

        }
    }
}
