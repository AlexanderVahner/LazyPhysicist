using System;

namespace LazyOptimizer.Model
{
    public interface IPlanCachedModel : IPlanBaseModel
    {
        DateTime CreationDate { get; }
        bool IsStarred { get; set; }
    }
}
