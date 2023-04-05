using System;
using System.Collections.Generic;

namespace LazyOptimizerDataService.DBModel
{
    public interface IPlansContext
    {
        List<CachedPlan> GetPlans(PlansFilterArgs args);
        void InsertPlans(IEnumerable<CachedPlan> plans);
        void UpdatePlan(CachedPlan plan);
        List<CachedObjective> GetObjectives(long planRowId);
        CachedNto GetNto(long planRowId);
        Vars GetVars();
        void UpdateVars(Vars vars);
        void ClearData(DateTime fromDate = default);
        bool Connected { get; set; }
    }
}
