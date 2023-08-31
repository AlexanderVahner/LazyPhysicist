using LazyContouring.Operations;
using LazyContouring.UI.Views;
using System.Windows.Controls;

namespace LazyContouring.UI.ViewModels
{
    public sealed class AsymmetricMarginOperationVM : OperationVM
    {
        public AsymmetricMarginOperationVM(OperationNode node, Border border) : base(node, border) { }

        protected override void InitBorder(Border border)
        {
            var marginControl = new AsymmetricMarginControl() { DataContext = Node.Operation as AsymmetricMarginOperation };
            border.Child = marginControl;
        }
    }

}
