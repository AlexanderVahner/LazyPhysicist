using System;
using System.Collections.Generic;

namespace LazyOptimizerDataService.DBModel
{
    public sealed class CachedPlan
    {
        public long RowId { get; set; }
        public string PlanId { get; set; }
        public string PatientId { get; set; }
        public string CourseId { get; set; }
        public DateTime CreationDate { get; set; }
        public long? FractionsCount { get; set; }
        public double? SingleDose { get; set; }
        public string MachineId { get; set; }
        public string Technique { get; set; }
        public long? SelectionFrequency { get; set; }
        public long? LDistance { get; set; }
        public string StructuresString { get; set; }
        public string Description { get; set; }
        public long? ApprovalStatus { get; set; }
        public long? Starred { get; set; }

        public List<CachedObjective> Objectives { get; set; }
        public CachedNto Nto { get; set; }
    }
}
