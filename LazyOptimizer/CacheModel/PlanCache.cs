using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.CacheModel
{
    public class PlanCache
    {
        public string PlanId;
        public string PatientId;
        public string CourseId;
        public long? FractionsCount;
        public double? SingleDose;
        public string MachineId;
        public long? Technique;
        public long? SelectionFrequency;
        public long? LDistance;
        public string StructuresString;
        public string Description;

        private List<ObjectiveCache> objectives;
        public List<ObjectiveCache> Objectives => objectives ?? (objectives = new List<ObjectiveCache>());

        private NtoCache nto;
        public NtoCache Nto => nto ?? (nto = new NtoCache());
    }
}
