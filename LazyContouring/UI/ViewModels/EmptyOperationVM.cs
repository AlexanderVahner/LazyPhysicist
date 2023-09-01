using LazyContouring.Operations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LazyContouring.UI.ViewModels
{
    public sealed class EmptyOperationVM : OperationVM
    {
        public EmptyOperationVM(OperationNode node) : base(node) { }

        protected override void InitUIElement()
        {
            UIElement = new TextBlock
            {
                Text = Node?.StructureVar?.StructureId ?? "*drop structure here*",
            };
        }
    }

}
