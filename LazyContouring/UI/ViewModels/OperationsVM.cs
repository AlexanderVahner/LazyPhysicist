using LazyContouring.Operations;
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

            var opString = new OperationStringVM(node);
            Grid.SetRow(opString.UIElement, grid.RowDefinitions.Count - 1);
            Grid.SetColumn(opString.UIElement, grid.ColumnDefinitions.Count - 1);
            grid.Children.Add(opString.UIElement);
        }

        public UIElement UIElement { get; set; }
    }
}
