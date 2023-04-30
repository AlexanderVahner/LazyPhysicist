using LazyPhysicist.Common;

namespace LazyOptimizer.UI.ViewModels
{
    public class ViewModel<TModel> : Notifier
    {
        public ViewModel(TModel sourceModel)
        {
            SourceModel = sourceModel;
        }
        public TModel SourceModel { get; }
    }
}
