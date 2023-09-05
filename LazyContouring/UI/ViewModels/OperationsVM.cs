using LazyContouring.Operations;
using LazyContouring.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace LazyContouring.UI.ViewModels
{
    public sealed class OperationsVM
    {
        private readonly Grid grid = new Grid();

        public OperationsVM() 
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
            var view = new ScrollViewer();
            view.Content = grid;

            UIElement = view;
        }

        public void AddOperationString(OperationNode node)
        {
            grid.RowDefinitions.Add(new RowDefinition());

            var opString = new OperationStringVM() { Node = node };
            var opStringUI = new OperationStringControl { DataContext = opString };
            Grid.SetRow(opStringUI, grid.RowDefinitions.Count - 1);
            Grid.SetColumn(opStringUI, grid.ColumnDefinitions.Count - 1);
            grid.Children.Add(opStringUI);
        }

        public UIElement UIElement { get; set; }
    }
}
