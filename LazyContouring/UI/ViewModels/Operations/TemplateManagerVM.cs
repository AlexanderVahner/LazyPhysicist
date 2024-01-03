using LazyContouring.Operations;
using LazyPhysicist.Common;
using System.Collections.ObjectModel;

namespace LazyContouring.UI.ViewModels.Operations
{
    public sealed class TemplateManagerVM : Notifier
    {
        public TemplateManagerVM(TemplateManager templateManager)
        {
            TemplateManager = templateManager;
        }

        public TemplateManager TemplateManager { get; }
        public ObservableCollection<OperationTemplate> CurrentTemplatesList { get; } = new ObservableCollection<OperationTemplate>();

    }
}
