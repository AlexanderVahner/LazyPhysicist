using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.App
{
    public class PlansFilterArgs
    {
        public string StructuresString { get; set; }
        public double SingleDose { get; set; } = .0;
        public int FractionsCount { get; set; } = 0;
        public string Technique { get; set; } = "";
        public string MachineId { get; set; } = "";
        public int Limit { get; set; } = 0;
    }
}
