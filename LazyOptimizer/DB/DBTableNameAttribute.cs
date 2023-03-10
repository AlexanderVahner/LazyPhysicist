using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.DB
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DBTableNameAttribute : Attribute
    {
        public string DBName;
        public DBTableNameAttribute(string dbName)
        {
            DBName = dbName;
        }
    }
}
