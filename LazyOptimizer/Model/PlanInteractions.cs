using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.Model
{
    public sealed class PlanInteractions : Notifier
    {
        private IPlanMergedModel planMergedModel;

        public void AddToMerged(IPlanBaseModel plan)
        {
            if (PlanMergedModel == null)
            {
                return;
            }
            PlanMergedModel?.Merge(plan);
        }
        public Func<IPlanMergedModel> CreateMergedPlan { get; set; }
        public IPlanMergedModel PlanMergedModel
        {
            get => planMergedModel ?? (planMergedModel = CreateMergedPlan?.Invoke());
            set => planMergedModel = value;
        }
    }
}
