﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarShips.Interfaces
{
    public interface IWeapon
    {
        bool IsLoaded{get;}
        string Fire();
        int Reload();
    }
}
