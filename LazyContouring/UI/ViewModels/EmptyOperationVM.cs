using LazyContouring.Operations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LazyContouring.UI.ViewModels
{
    public sealed class EmptyOperationVM : OperationVM
    {
        public EmptyOperationVM(OperationNode node, Border border) : base(node, border) { }

        protected override void InitBorder(Border border)
        {
            if (Node == null)
            {
                border.CornerRadius = new CornerRadius(10);
                border.Child = new TextBlock
                {
                    Text = "+",
                };
                return;
            }

            border.BorderBrush = new SolidColorBrush(Node?.StructureVar?.Color ?? Color.FromRgb(0, 0, 0));
            border.BorderThickness = new Thickness(3);
            border.Child = new TextBlock
            {
                Text = Node?.StructureVar?.StructureId ?? "*drop structure here*",
            };
        }
    }

}
