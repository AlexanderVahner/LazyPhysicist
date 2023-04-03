using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizerDataService.DBModel
{
    public class CachedNto
    {
        public long RowId { get; set; }
        public long PlanRowId { get; set; }
        public bool IsAutomatic { get; set; } = true;
        public double? DistanceFromTargetBorderInMM { get; set; }
        public double? StartDosePercentage { get; set; }
        public double? EndDosePercentage { get; set; }
        public double? FallOff { get; set; }
        public double? Priority { get; set; } = 100.0;
    }
}
