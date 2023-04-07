using ESAPIInfo.Plan;
using LazyOptimizerDataService.DB;
using LazyOptimizerDataService.DBModel;
using System;

namespace LazyOptimizer.App
{
    /// <summary>
    /// Poorman's DI Container
    /// </summary>
    public class AppContext : IDisposable
    {
        public IPlanInfo CurrentPlan { get; set; }
        public Settings Settings { get; set; }
        public IDbService DbService { get; set; }
        public IPlansContext PlansContext { get; set; }
        public PlansFilterArgs PlansFilterArgs { get; set; }

        public void Dispose()
        {
            DbService?.Dispose();
            Settings?.Save();
        }
    }
}
