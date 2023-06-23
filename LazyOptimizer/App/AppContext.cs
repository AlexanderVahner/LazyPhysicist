using ESAPIInfo.Plan;
using LazyOptimizerDataService.DB;
using LazyOptimizerDataService.DBModel;
using System;

namespace LazyOptimizer.App
{
    /// <summary>
    /// Poorman's DI Container
    /// </summary>
    public class AppContext
    {
        public IPlanInfo CurrentPlan { get; set; }
        public GeneralSettings GeneralSettings { get; set; }
        public UserSettings UserSettings { get; set; }
        public IDbService DbService { get; set; }
        public IPlansContext PlansContext { get; set; }
        public PlansFilterArgs PlansFilterArgs { get; set; }
    }
}
