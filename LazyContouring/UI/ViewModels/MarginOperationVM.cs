using LazyContouring.Operations;
using LazyContouring.UI.Views;
using System.Windows.Controls;

namespace LazyContouring.UI.ViewModels
{
    public sealed class MarginOperationVM : OperationVM
    {
        public MarginOperationVM(OperationNode node, Border border) : base(node, border) { }

        protected override void InitBorder(Border border)
        {
            var marginControl = new MarginOperationControl() { DataContext = Node.Operation as MarginOperation };
            border.Child = marginControl;
        }
    }

}
