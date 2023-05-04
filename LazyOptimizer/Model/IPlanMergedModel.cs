using System.Collections.Generic;

namespace LazyOptimizer.Model
{
    public interface IPlanMergedModel : IPlanBaseModel
    {
        void Merge(IPlanBaseModel plan);
        IEnumerable<IPlanBaseModel> MergedPlans { get; }
    }
}
