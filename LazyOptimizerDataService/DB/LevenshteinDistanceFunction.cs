using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizerDataService.DB
{
    [SQLiteFunction(Name = "Levenshtein", Arguments = 2, FuncType = FunctionType.Scalar)]
    public class LevenshteinDistanceFunction : SQLiteFunction
    {
        public override object Invoke(object[] args)
        {
            return Levenshtein.ComputeDistance(args[0].ToString(), args[1].ToString());
        }
    }
}
