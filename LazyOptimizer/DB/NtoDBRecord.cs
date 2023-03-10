using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.DB
{
    [DBTableName("NTO")]
    public class NtoDBRecord : DBRecord
    {
        public NtoDBRecord(SQLiteService dbService) : base(dbService, "NTO") { }

        public long PlanRowId;
        public long IsAutomatic;
        public double? DistanceFromTargetBorderInMM;
        public double? StartDosePercentage;
        public double? EndDosePercentage;
        public double? FallOff;
        public double? Priority;
    }
}
