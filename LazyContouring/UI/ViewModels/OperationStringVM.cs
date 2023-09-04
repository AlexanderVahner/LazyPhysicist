using LazyContouring.Operations;
using LazyContouring.UI.Views;
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

            var nodeVM = new OperationNodeVM() { Node = node };
            var stringUI = new OperationNodeControl() { VM = nodeVM };

            Grid.SetRow(stringUI, 0);
            Grid.SetColumn(stringUI, 0);
            grid.Children.Add(stringUI);

            UIElement = grid;
        }

        public UIElement UIElement { get; set; }
    }
}
