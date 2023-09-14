using LazyContouring.Operations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyContouring.Operations
{
    public sealed class TemplateManager
    {
        private ObservableCollection<OperationTemplate> manualTemplates = new ObservableCollection<OperationTemplate>();
        private ObservableCollection<OperationTemplate> automaticTemplates = new ObservableCollection<OperationTemplate>();

        public ObservableCollection<OperationTemplate> ManualTemplates { get => manualTemplates; set => manualTemplates = value; }
        public ObservableCollection<OperationTemplate> AutomaticTemplates { get => automaticTemplates; set => automaticTemplates = value; }
    }
}
