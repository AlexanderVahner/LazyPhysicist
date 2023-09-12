using LazyOptimizer.App;
using LazyPhysicist.Common;

namespace LazyOptimizer.UI.ViewModels
{
    public sealed class SettingsVM : Notifier
    {
        public SettingsVM()
        {

        }
        public UserSettings Settings { get; set; }
    }
}
