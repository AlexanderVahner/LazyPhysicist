using LazyContouring.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LazyContouring.UI.ViewModels
{
    public sealed class OperationStringVM
    {
        private readonly OperationNode node;
        private readonly Grid grid = new Grid();

        public OperationStringVM(OperationNode node)
        {
            this.node = node;

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.VerticalAlignment = VerticalAlignment.Top;
            grid.HorizontalAlignment = HorizontalAlignment.Left;

            var nodeVM = new OperationNodeVM();
            nodeVM.Node = node;
            Grid.SetRow(nodeVM.UIElement, 0);
            Grid.SetColumn(nodeVM.UIElement, 0);
            grid.Children.Add(nodeVM.UIElement);

            UIElement = grid;
        }

        public UIElement UIElement { get; set; }
    }
}
