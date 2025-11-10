using LazyContouring.Operations;
using LazyContouring.Operations.ContextConditions;
using LazyContouring.UI.ViewModels.Operations.ContextConditions;
using LazyContouring.UI.Views;
using LazyPhysicist.Common;
using System.Windows.Controls;

namespace LazyContouring.UI.ViewModels
{
    public sealed class OperationTemplateVM : Notifier
    {
        private readonly OperationTemplate operationTemplate;

        private readonly ConditionNodeVM conditionNodeVM;

        public OperationTemplateVM(OperationTemplate operationTemplate)
        {
            this.operationTemplate = operationTemplate;

            conditionNodeVM = new ConditionNodeVM(operationTemplate.ConditionNodes[0]);
            var operationsVM = new OperationsVM();
            foreach (var node in operationTemplate.OperationNodes)
            {
                operationsVM.AddOperationString(node);
            }

            OperationPage = new OperationPage() { DataContext = operationsVM };
        }

        public MetaCommand AddGroupCommand => new MetaCommand(
            o => ConditionNodeVM.Node.Children.Add(new ConditionGroup())
        );

        public OperationTemplate OperationTemplate => operationTemplate;
        public ConditionNodeVM ConditionNodeVM => conditionNodeVM;
        public Page OperationPage { get; private set; }
    }
}
