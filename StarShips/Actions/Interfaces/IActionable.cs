using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarShips.Interfaces
{
    interface IActionable
    {
        string DoAction(Ship target);
    }
}
