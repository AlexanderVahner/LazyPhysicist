using LazyOptimizerDataService.DB;
using LazyOptimizerDataService.DBModel;
using System;
using System.Collections.Generic;

namespace LazyOptimizerDataService
{
    /*public interface IDataService : IDisposable
    {
        List<PlanDBRecord> DBPlans { get; }
        string DBPath { get; }
        bool Connected { get; set; }
        void GetPlans(PlansFilterArgs args);
        void GetObjectives(IList<ObjectiveDBRecord> destination, long PlanRowId);
        NtoDBRecord GetNto(long PlanRowId);
        //void SavePlanToDB(PlanInfo plan);
        void ClearData(DateTime fromDate = default);
        DateTime? GetLastCheckDate();
        void SetLastCheckDate(DateTime date);
        void IncreasePlanSelectionFrequency(long rowId);

        event EventHandler<List<PlanDBRecord>> PlansSelected;
    }*/
}
