using LazyContouring.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LazyContouring.UI.ViewModels
{
    public sealed class OperationNodeVM
    {
        private readonly Grid grid = new Grid();
        private readonly Canvas leftNodeArrows = new Canvas();
        private readonly Canvas rightNodeArrows = new Canvas();
        private readonly Brush arrowsBrush = new SolidColorBrush(Colors.Black);
        
        private readonly ColumnDefinition colDef1 = new ColumnDefinition();
        private readonly ColumnDefinition colDef2 = new ColumnDefinition();
        private readonly RowDefinition rowDef1 = new RowDefinition();
        private readonly RowDefinition rowDef2 = new RowDefinition();

        private const double arrowsThickness = 1.0;
        private const int arrowsMarginY = 20;
        private const int arrowsMarginX = 20;

        private bool leftNodeOnlyNedded = false;
        private OperationNode node;

        public OperationNodeVM(OperationNode node)
        {
            grid.ColumnDefinitions.Add(colDef1);
            grid.ColumnDefinitions.Add(colDef2);
            grid.RowDefinitions.Add(rowDef1);
            grid.RowDefinitions.Add(rowDef2);

            leftNodeArrows.SizeChanged += LeftNodeArrows_SizeChanged;
            rightNodeArrows.SizeChanged += RightNodeArrows_SizeChanged;
            AddIntoGrid(leftNodeArrows, 0, 0);
            AddIntoGrid(rightNodeArrows, 1, 0);

            /*testButton = new Button 
            { 
                Content = node.Operation.OperationType.ToString(), 
                Margin = new Thickness(10),
                Padding = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            AddIntoGrid(testButton, 0, 0, rowSpan: 2);*/
            

            Node = node;
            UIElement = grid;
        }

        public void Init()
        {
            
        }

        private void SetNode(OperationNode node)
        {
            this.node = node;

            leftNodeOnlyNedded = node.Operation.LeftNodeOnlyNedded;
            AddIntoGrid(OperationVM.CreateOperationVM(node).UIElement, 0, 0, rowSpan: 2);
            //testButton.Content = node.Operation.OperationType == OperationType.Empty ? (node.StructureVar?.StructureId ?? "Empty") : node.Operation.OperationType.ToString();
            grid.RowDefinitions[1].Height = (leftNodeOnlyNedded == true) ? new GridLength(0) : new GridLength(1, GridUnitType.Star);

            if (node.NodeLeft != null)
            {
                var nodeVM = new OperationNodeVM(node.NodeLeft);
                AddIntoGrid(nodeVM.UIElement, 0, 1);
            }

            if (node.NodeRight != null)
            {
                var nodeVM = new OperationNodeVM(node.NodeRight);
                AddIntoGrid(nodeVM.UIElement, 1, 1);
            }
        }

        private void AddIntoGrid(UIElement element, int row, int column, int rowSpan = 0, int columnSpan = 0)
        {
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
            if (rowSpan > 1)
            {
                Grid.SetRowSpan(element, rowSpan);
            }
            if (columnSpan > 1)
            {
                Grid.SetColumnSpan(element, columnSpan);
            }
            grid.Children.Add(element);
        }

        private void LeftNodeArrows_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Equals(e.NewSize.Width, double.NaN) || Equals(e.NewSize.Height, double.NaN))
            {
                return;
            }

            leftNodeArrows.Children.Clear();
            var line = new Line
            {
                X1 = node.IsRootNode ? arrowsMarginX : 0,
                Y1 = arrowsMarginY,
                X2 = e.NewSize.Width,
                Y2 = arrowsMarginY,
                Stroke = arrowsBrush,
                StrokeThickness = arrowsThickness
            };

            leftNodeArrows.Children.Add(line);

            if (!leftNodeOnlyNedded)
            {
                var lineVert = new Line
                {
                    X1 = e.NewSize.Width / 2,
                    Y1 = arrowsMarginY,
                    X2 = e.NewSize.Width / 2,
                    Y2 = e.NewSize.Height,
                    Stroke = arrowsBrush,
                    StrokeThickness = arrowsThickness
                };

                leftNodeArrows.Children.Add(lineVert);
            }
        }

        private void RightNodeArrows_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Equals(e.NewSize.Width, double.NaN) || Equals(e.NewSize.Height, double.NaN))
            {
                return;
            }

            rightNodeArrows.Children.Clear();

            if (leftNodeOnlyNedded)
            {
                return;
            }

            var line = new Line
            {
                X1 = e.NewSize.Width / 2,
                Y1 = arrowsMarginY,
                X2 = e.NewSize.Width,
                Y2 = arrowsMarginY,
                Stroke = arrowsBrush,
                StrokeThickness = arrowsThickness
            };

            rightNodeArrows.Children.Add(line);

            var lineVert = new Line
            {
                X1 = e.NewSize.Width / 2,
                Y1 = 0,
                X2 = e.NewSize.Width / 2,
                Y2 = arrowsMarginY,
                Stroke = arrowsBrush,
                StrokeThickness = arrowsThickness
            };

            rightNodeArrows.Children.Add(lineVert);
        }

        public OperationNode Node 
        {
            get => node;
            set => SetNode(value);
        }
        public UIElement UIElement { get; set; }
    }
}
