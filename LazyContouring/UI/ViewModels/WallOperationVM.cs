using LazyContouring.Operations;
using LazyContouring.UI.Views;
using System.Windows.Controls;

namespace LazyContouring.UI.ViewModels
{
    public sealed class WallOperationVM : OperationVM
    {
        public WallOperationVM(OperationNode node, Border border) : base(node, border) { }

        protected override void InitBorder(Border border)
        {
            var wallControl = new WallOperationControl() { DataContext = Node.Operation as WallOperation };
            border.Child = wallControl;
        }
    }

}
