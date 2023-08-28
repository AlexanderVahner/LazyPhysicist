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
        private const double arrowsThickness = 1.0;
        private const int arrowsMarginY = 20;
        private const int arrowsMarginX = 20;
        private readonly Thickness defaultBorderMargin = new Thickness(0, 5, 22, 5);
        private const double insertPlaceSize = 18;
        private const double insertPlaceEllipseSize = 10;
        private const double insertPlaceEllipsePosition = 4;
        private const double insertPlaceFromRightPosition = 20;
        private const double insertPlaceFromTopPosition = 11;


        private readonly Grid grid = new Grid();
        private readonly Canvas leftNodeArrows = new Canvas();
        private readonly Canvas rightNodeArrows = new Canvas();
        private readonly Canvas leftNodeInsertPlace = new Canvas() { Width = insertPlaceSize, Height = insertPlaceSize, AllowDrop = true };
        private readonly Canvas rightNodeInsertPlace = new Canvas() { Width = insertPlaceSize, Height = insertPlaceSize, AllowDrop = true };
        private readonly Brush arrowsBrush = new SolidColorBrush(Colors.Black);
        
        
        private readonly ColumnDefinition colDef1 = new ColumnDefinition();
        private readonly ColumnDefinition colDef2 = new ColumnDefinition();
        private readonly RowDefinition rowDef1 = new RowDefinition();
        private readonly RowDefinition rowDef2 = new RowDefinition();

        

        private bool leftNodeOnlyNedded = false;
        private OperationNode node;

        public OperationNodeVM(OperationNode node)
        {
            grid.ColumnDefinitions.Add(colDef1);
            grid.ColumnDefinitions.Add(colDef2);
            grid.RowDefinitions.Add(rowDef1);
            grid.RowDefinitions.Add(rowDef2);

            var leftEllipse = new Ellipse
            {
                Width = insertPlaceEllipseSize,
                Height = insertPlaceEllipseSize,
                Stroke = arrowsBrush,
                StrokeThickness = arrowsThickness,
                Fill = Brushes.White
            };

            Canvas.SetTop(leftEllipse, insertPlaceEllipsePosition);
            Canvas.SetLeft(leftEllipse, insertPlaceEllipsePosition);

            var rightEllipse = new Ellipse
            {
                Width = insertPlaceEllipseSize,
                Height = insertPlaceEllipseSize,
                Stroke = arrowsBrush,
                StrokeThickness = arrowsThickness,
                Fill = Brushes.White
            };

            Canvas.SetTop(rightEllipse, insertPlaceEllipsePosition);
            Canvas.SetLeft(rightEllipse, insertPlaceEllipsePosition);

            leftNodeInsertPlace.Children.Add(leftEllipse);
            rightNodeInsertPlace.Children.Add(rightEllipse);

            leftNodeArrows.SizeChanged += LeftNodeArrows_SizeChanged;
            rightNodeArrows.SizeChanged += RightNodeArrows_SizeChanged;
            AddIntoGrid(leftNodeArrows, 0, 0);
            AddIntoGrid(rightNodeArrows, 1, 0);
            

            Node = node;
            UIElement = grid;
        }

        public void InitUI()
        {
            
        }

        private void SetNode(OperationNode node)
        {
            this.node = node;

            if (node == null)
            {
                return;
            }

            leftNodeOnlyNedded = node.Operation.LeftNodeOnlyNedded;

            var border = CreateDefaultBorder();
            OperationVM = OperationVM.CreateOperationVM(node, border);
            
            AddIntoGrid(border, 0, 0, rowSpan: 2);
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
                X1 = 0,
                Y1 = arrowsMarginY,
                X2 = Node.NodeLeft != null ? e.NewSize.Width : e.NewSize.Width - insertPlaceFromRightPosition / 2,
                Y2 = arrowsMarginY,
                Stroke = arrowsBrush,
                StrokeThickness = arrowsThickness
            };

            leftNodeArrows.Children.Add(line);

            Canvas.SetLeft(leftNodeInsertPlace, e.NewSize.Width - insertPlaceFromRightPosition);
            Canvas.SetTop(leftNodeInsertPlace, insertPlaceFromTopPosition);

            leftNodeArrows.Children.Add(leftNodeInsertPlace);

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
                X2 = Node.NodeRight != null ? e.NewSize.Width : e.NewSize.Width - insertPlaceFromRightPosition / 2,
                Y2 = arrowsMarginY,
                Stroke = arrowsBrush,
                StrokeThickness = arrowsThickness
            };

            rightNodeArrows.Children.Add(line);

            Canvas.SetLeft(rightNodeInsertPlace, e.NewSize.Width - insertPlaceFromRightPosition);
            Canvas.SetTop(rightNodeInsertPlace, insertPlaceFromTopPosition);
            rightNodeArrows.Children.Add(rightNodeInsertPlace);

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

        private Border CreateDefaultBorder()
        {
            return new Border
            {
                CornerRadius = new CornerRadius(5),
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Background = Brushes.White,
                Margin = defaultBorderMargin,
                Padding = new Thickness(3),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                AllowDrop = true
            };
        }

        public OperationNode Node 
        {
            get => node;
            set => SetNode(value);
        }

        public OperationVM OperationVM { get; private set; }

        public UIElement UIElement { get; set; }
    }
}
