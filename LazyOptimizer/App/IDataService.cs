using ESAPIInfo.Plan;
using LazyOptimizer.DB;
using System;
using System.Collections.Generic;

namespace LazyOptimizer.App
{
    public interface IDataService : IDisposable
    {
        List<PlanDBRecord> DBPlans { get; }
        string DBPath { get; }
        bool Connected { get; set; }
        void GetPlans(PlansFilterArgs args);
        void GetObjectives(IList<ObjectiveDBRecord> destination, long PlanRowId);
        NtoDBRecord GetNto(long PlanRowId);
        void SavePlanToDB(PlanInfo plan);
        void ClearData();
        DateTime? GetLastCheckDate();
        void SetLastCheckDate(DateTime date);
    }
}
