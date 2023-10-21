using LazyContouring.Models;
using LazyContouring.Operations;
using LazyContouring.Operations.ContextConditions;
using LazyContouring.UI.ViewModels.Operations.ContextConditions;
using LazyContouring.UI.Views.ContextConditionControls;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace LazyContouring.UI.ViewModels
{
    public sealed class OperationTemplateVM : Notifier
    {
        private readonly OperationTemplate operationTemplate;
        private ConditionNodeVM selectedNodeVM;
        private ConditionNodeVM conditionNodeVM;

        public OperationTemplateVM(OperationTemplate operationTemplate)
        {
            this.operationTemplate = operationTemplate;
            selectedNodeVM = ConditionNodeVM;
        }

        public ConditionGroup GetCurrentGroup()
        {
            if (SelectedNodeVM.Node is ContextCondition)
            {
                return (ConditionGroup)SelectedNodeVM.ParentGroup;
            }
            else if (SelectedNodeVM.Node is ConditionGroup cg)
            {
                return cg;
            }

            return null;
        }

        public MetaCommand AddGroupCommand => new MetaCommand(
            o => GetCurrentGroup()?.Children.Add(new ConditionGroup()),
            o => SelectedNodeVM != null
        );

        public MetaCommand AddStructureConditionCommand => new MetaCommand(
            o => GetCurrentGroup()?.Children.Add(new StructureCondition()),
            o => SelectedNodeVM != null
        );

        public MetaCommand AddDiagnosisConditionCommand => new MetaCommand(
            o => GetCurrentGroup()?.Children.Add(new DiagnosisCondition()),
            o => SelectedNodeVM != null
        );

        public MetaCommand AddImageConditionCommand => new MetaCommand(
            o => GetCurrentGroup()?.Children.Add(new ImageCondition()),
            o => SelectedNodeVM != null
        );

        public MetaCommand RemoveConditionCommand => new MetaCommand(
            o => SelectedNodeVM.ParentGroup?.Children.Remove(SelectedNodeVM.Node),
            o => SelectedNodeVM?.ParentGroup != null
        );

        public OperationTemplate OperationTemplate => operationTemplate;
        public ConditionNodeVM ConditionNodeVM => conditionNodeVM ?? (conditionNodeVM = new ConditionNodeVM(operationTemplate.RootConditionGroup, null));
        public ConditionNodeVM SelectedNodeVM
        {
            get => selectedNodeVM;
            set
            {
                if (value == null && ConditionNodeVM.ChildrenConditionNodes.Count > 0)
                {
                    value = ConditionNodeVM;
                }

                SetProperty(ref selectedNodeVM, value);
            }
        }
    }
}
