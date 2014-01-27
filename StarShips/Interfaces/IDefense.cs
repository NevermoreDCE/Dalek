using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarShips.Interfaces
{
    public interface IDefense
    {
        DefenseResult TakeHit(int Damage);
    }
}
