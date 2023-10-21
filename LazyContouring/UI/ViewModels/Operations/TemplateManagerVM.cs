using LazyContouring.Operations;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
