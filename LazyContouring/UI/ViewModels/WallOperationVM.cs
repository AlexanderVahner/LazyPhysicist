using LazyContouring.Operations;
using LazyContouring.UI.Views;
using System.Windows.Controls;

namespace LazyContouring.UI.ViewModels
{
    public sealed class WallOperationVM : OperationVM
    {
        public WallOperationVM(OperationNode node) : base(node) { }

        protected override void InitUIElement()
        {
            UIElement = new WallOperationControl() { DataContext = Node.Operation as WallOperation };
        }
    }

}
