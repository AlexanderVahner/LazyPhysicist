using LazyContouring.Operations;
using LazyContouring.Operations.ContextConditions;
using LazyContouring.UI.ViewModels.Operations.ContextConditions;
using LazyPhysicist.Common;

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
        }

        public MetaCommand AddGroupCommand => new MetaCommand(
            o => ConditionNodeVM.Node.Children.Add(new ConditionGroup())
        );

        public OperationTemplate OperationTemplate => operationTemplate;
        public ConditionNodeVM ConditionNodeVM => conditionNodeVM;

    }
}
