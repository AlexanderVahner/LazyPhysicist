using LazyContouring.Models;
using LazyContouring.Operations;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
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

        private Grid grid;
        private ColumnDefinition colDef1;
        private ColumnDefinition colDef2;
        private RowDefinition rowDef1;
        private RowDefinition rowDef2;

        private Border mainBorder;
        private Canvas leftNodeArrows;
        private Canvas rightNodeArrows;
        private Canvas leftNodeInsertPlace;
        private Canvas rightNodeInsertPlace;
        private Brush arrowsBrush;

        private bool leftNodeOnlyNedded = false;
        private OperationNode node;
        private OperationNodeVM nodeLeftVM;
        private OperationNodeVM nodeRightVM;

        public OperationNodeVM()
        {
            
        }

        private void InitEmptyUI()
        {
            var emptyBorder = CreateDefaultBorder();
            emptyBorder.Width = 30;
            emptyBorder.Height = 30;
            emptyBorder.Child = new TextBlock { Text = "+" };
            UIElement = emptyBorder;
        }

        private void InitFullUI()
        {
            grid = new Grid();
            colDef1 = new ColumnDefinition();
            colDef2 = new ColumnDefinition();
            rowDef1 = new RowDefinition();
            rowDef2 = new RowDefinition();

            leftNodeArrows = new Canvas();
            rightNodeArrows = new Canvas();
            leftNodeInsertPlace = new Canvas() { Width = insertPlaceSize, Height = insertPlaceSize, AllowDrop = true };
            rightNodeInsertPlace = new Canvas() { Width = insertPlaceSize, Height = insertPlaceSize, AllowDrop = true };
            arrowsBrush = new SolidColorBrush(Colors.Black);

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

            leftNodeInsertPlace.Drop += LeftInsertionDrop;
            rightNodeInsertPlace.Drop += RightInsertionDrop;

            leftNodeArrows.SizeChanged += LeftNodeArrows_SizeChanged;
            rightNodeArrows.SizeChanged += RightNodeArrows_SizeChanged;
            AddIntoGrid(leftNodeArrows, 0, 0);
            AddIntoGrid(rightNodeArrows, 1, 0);

            mainBorder = CreateDefaultBorder();
            AddIntoGrid(mainBorder, 0, 0, rowSpan: 2);

            UIElement = grid;
        }

        public void DrawOperation()
        {
            mainBorder.Child = null;

            if (node == null)
            {
                return;
            }

            OperationVM = OperationVM.CreateOperationVM(node, mainBorder);
            grid.RowDefinitions[1].Height = (leftNodeOnlyNedded == true) ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
        }

        public void DrawKids()
        {
            DrawKid(ref nodeLeftVM, node.NodeLeft, 0, 1);
            DrawKid(ref nodeRightVM, node.NodeRight, 1, 1);
        }

        private void DrawKid(ref OperationNodeVM kid, OperationNode node, int gridRow, int gridColumn)
        {
            if (kid?.UIElement != null)
            {
                grid.Children.Remove(kid.UIElement);
            }

            if (node != null)
            {
                kid = new OperationNodeVM();
                kid.Node = node;
                AddIntoGrid(kid.UIElement, gridRow, gridColumn);
            }
        }

        /*public void DrawAll()
        {
            Draw();
            DrawKids();
            NodeLeftVM?.DrawAll();
            NodeRightVM?.DrawAll();
        }*/

        private void SetNode(OperationNode node)
        {
            this.node = node;

            if (node == null)
            {
                return;
            }

            leftNodeOnlyNedded = node.Operation.LeftNodeOnlyNedded;

            DrawOperation();

            DrawKids();
        }

        public void InsertBeforeNode(OperationNode node, ref OperationNodeVM beforeThisNode)
        {
            if (node == null)
            {
                return;
            }

            node.InsertNodeBefore(node, ref beforeThisNode.node);
            beforeThisNode.Node = node;

        }

        private OperationNode CreateNodeFromDrop(object drop)
        {
            OperationNode node = null;
            Operation operation;

            if (drop is string stringDrop)
            {
                operation = OperationCreator.CreateFromString(stringDrop);
                if (operation != null)
                {
                    node = new OperationNode { Operation = operation };
                }
            }
            else if (drop is StructureVariable strVar)
            {
                node = new OperationNode { StructureVar = strVar, Operation = new EmptyOperation() };
            }
            return node;
        }

        private void LeftInsertionDrop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.Text) ?? e.Data.GetData(typeof(StructureVariable));
            if (data == null)
            {
                return;
            }

            if (nodeLeftVM == null)
            {
                node.NodeLeft = CreateNodeFromDrop(data);
                nodeLeftVM = new OperationNodeVM();
                nodeLeftVM.Node = node.NodeLeft;
                return;
            }

            InsertBeforeNode(CreateNodeFromDrop(data), ref nodeLeftVM);
        }

        private void RightInsertionDrop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.Text) ?? e.Data.GetData(typeof(StructureVariable));
            if (data == null)
            {
                return;
            }

            if (nodeRightVM == null)
            {
                node.NodeRight = CreateNodeFromDrop(data);
                nodeRightVM = new OperationNodeVM();
                nodeRightVM.Node = node.NodeRight;
                return;
            }

            InsertBeforeNode(CreateNodeFromDrop(data), ref nodeRightVM);
        }

        private void AddIntoGrid(UIElement element, int row, int column, int rowSpan = 0, int columnSpan = 0)
        {
            if (element == null)
            {
                return;
            }

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
                MinHeight = 20,
                MinWidth = 20,
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
        public OperationNodeVM NodeLeftVM { get => nodeLeftVM; private set => nodeLeftVM = value; }
        public OperationNodeVM NodeRightVM { get => nodeRightVM; private set => nodeRightVM = value; }

        public UIElement UIElement { get; set; }
    }
}
