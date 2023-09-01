using LazyContouring.Operations;
using LazyContouring.UI.Views;
using System.Windows.Controls;

namespace LazyContouring.UI.ViewModels
{
    public sealed class AsymmetricMarginOperationVM : OperationVM
    {
        public AsymmetricMarginOperationVM(OperationNode node) : base(node) { }

        protected override void InitUIElement()
        {
            UIElement = new AsymmetricMarginControl() { DataContext = Node.Operation as AsymmetricMarginOperation };
        }
    }

}
