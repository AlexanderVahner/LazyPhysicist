using ESAPIInfo.Patients;
using ESAPIInfo.Plan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.App
{
    public class AppContext : IDisposable
    {
        public PatientInfo Patient { get; set; }
        public PlanInfo Plan { get; set; }
        public Settings Settings { get; set; }
        public IDataService DataService { get; set; }
        public PlansFilterArgs PlansFilterArgs { get; set; }

        public void Dispose()
        {
            Settings?.Save();
            DataService?.Dispose();
        }
    }
}
