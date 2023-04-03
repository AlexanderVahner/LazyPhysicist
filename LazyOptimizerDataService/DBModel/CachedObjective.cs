using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizerDataService.DBModel
{
    public class CachedObjective
    {
        public long RowId { get; set; }
        public long PlanRowId { get; set; }
        public string StructureId { get; set; }
        public long? ObjType { get; set; }
        public double? Priority { get; set; }
        public long? Operator { get; set; }
        public double? Dose { get; set; }
        public double? Volume { get; set; }
        public double? ParameterA { get; set; }
    }
}
