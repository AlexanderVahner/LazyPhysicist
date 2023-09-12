using LazyOptimizer.App;
using LazyPhysicist.Common;

namespace LazyOptimizer.UI.ViewModels
{
    public sealed class CheckPlansVM : Notifier
    {
        public CheckPlansVM()
        {

        }
        public MainVM MainVM { get; set; }
        public UserSettings Settings { get; set; }
    }
}
