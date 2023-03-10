using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.DB
{
    public class EventDelegates
    {
        public delegate void DBFieldChanged(string planRowId, string description);
    }
}
