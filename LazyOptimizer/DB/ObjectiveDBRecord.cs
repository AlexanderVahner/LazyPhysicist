using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.DB
{
    [DBTableName("Objectives")]
    public class ObjectiveDBRecord : DBRecord
    {
        public ObjectiveDBRecord(SQLiteService dbService) : base(dbService, "Objectives") { }

        public long PlanRowId;
        public string StructureId;
        public long? ObjType;
        public double? Priority;
        public long? Operator;
        public double? Dose;
        public double? Volume;
        public double? ParameterA;
    }
}
