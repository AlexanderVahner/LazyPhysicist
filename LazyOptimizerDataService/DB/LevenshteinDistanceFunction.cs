using LazyPhysicist.Common;
using System.Data.SQLite;

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
