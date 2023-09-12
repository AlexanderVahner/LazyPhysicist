using LazyContouring.Operations;
using LazyContouring.UI.Views;

namespace LazyContouring.UI.ViewModels
{
    public sealed class AssignOperationVM : OperationVM
    {
        private StructureVariableVM structureVariableVM;

        public AssignOperationVM(OperationNode node) : base(node)
        {
            StructureVariableVM = new StructureVariableVM(node.StructureVar);
            node.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(node.StructureVar))
                {
                    StructureVariableVM.StructureVariable = node.StructureVar;
                }
            };
        }

        protected override void InitUIElement()
        {
            UIElement = new AssignOperationControl() { DataContext = this };
        }

        public StructureVariableVM StructureVariableVM { get => structureVariableVM; set => SetProperty(ref structureVariableVM, value); }
    }

}
