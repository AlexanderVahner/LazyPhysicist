using LazyContouring.Operations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LazyContouring.UI.ViewModels
{
    public sealed class AssignOperationVM : OperationVM
    {
        public AssignOperationVM(OperationNode node) : base(node) { }

        protected override void InitUIElement()
        {
            UIElement = new TextBlock
            {
                Text = (Node?.StructureVar?.StructureId ?? "*drop structure here*") + " = ",
            };
        }
    }

}
