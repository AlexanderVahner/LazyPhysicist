using LazyContouring.Operations;
using LazyContouring.Operations.ContextConditions;
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

        public OperationTemplate CreateTemplate(IEnumerable<OperationNode> nodes)
        {
            OperationTemplate template = new OperationTemplate();
            template.ChildrenConditionNodes.Add(new ConditionGroup());

            foreach (OperationNode node in nodes)
            {
                template.RootNodes.Add((OperationNode)node.Clone());
            }

            return template.RootNodes.Count == 0 ? null : template;
        }

        public void SaveTemplate(OperationTemplate template) 
        {
            if (template == null || template.RootNodes.Count == 0) 
            { 
                return; 
            }

            automaticTemplates.Remove(template);
            manualTemplates.Remove(template);

            if (template.IsAutomatic)
            {
                automaticTemplates.Add(template);
            }
            else
            {
                manualTemplates.Add(template);
            }
        }

        public ObservableCollection<OperationTemplate> ManualTemplates { get => manualTemplates; set => manualTemplates = value; }
        public ObservableCollection<OperationTemplate> AutomaticTemplates { get => automaticTemplates; set => automaticTemplates = value; }
    }
}
