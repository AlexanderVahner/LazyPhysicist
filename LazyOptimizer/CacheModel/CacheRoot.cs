using LazyOptimizer.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.CacheModel
{
    public class CacheRoot
    {
        private List<PlanCache> plans;
        private Func<PlansFilterArgs, List<PlanCache>> getPlans;
        private PlansFilterArgs filter;
        public CacheRoot(Func<PlansFilterArgs, List<PlanCache>> getPlans, PlansFilterArgs filter)
        {
            this.getPlans = getPlans;
            this.filter = filter;

            if (filter != null)
            {
                filter.PropertyChanged += (s, e) => UpdatePlans();
            }
        }
        public void UpdatePlans()
        {
            plans = getPlans?.Invoke(filter);
            PlansUpdated?.Invoke(this, EventArgs.Empty);
        }
        public List<PlanCache> Plans
        {
            get
            {
                if (plans == null)
                {
                    UpdatePlans();
                }
                return plans;
            }
        }

        public event EventHandler PlansUpdated;
        
    }
}
