﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarShips.Orders.Interfaces
{
    public interface IMoveOrder
    {
        string ExecuteOrder(Ship ship, int impulse);
    }
}
