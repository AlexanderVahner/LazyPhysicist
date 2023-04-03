using ESAPIInfo.Plan;
using LazyOptimizerDataService;
using LazyOptimizerDataService.DB;
using LazyOptimizerDataService.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.App
{
    /// <summary>
    /// Poorman's DI Container
    /// </summary>
    public class AppContext : IDisposable
    {
        public PlanInfo CurrentPlan { get; set; }
        public Settings Settings { get; set; }
        public IDbService DbService { get; set; }
        public IPlansContext PlansContext { get; set; }
        public PlansFilterArgs PlansFilterArgs { get; set; }

        public void Dispose()
        {
            Settings?.Save();
            //DataService?.Dispose();
        }
    }
}
