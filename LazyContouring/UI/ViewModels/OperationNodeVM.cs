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
        private const double insertPlaceSize = 18;
        private const double insertPlaceEllipseSize = 10;
        private const double insertPlaceEllipsePosition = 4;
        private const double insertPlaceFromRightPosition = 20;
        private const double insertPlaceFromTopPosition = 11;

        private readonly Grid grid = new Grid() { VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left };
        private readonly ColumnDefinition colDef1 = new ColumnDefinition();
        private readonly ColumnDefinition colDef2 = new ColumnDefinition();
        private readonly RowDefinition rowDef1 = new RowDefinition();
        private readonly RowDefinition rowDef2 = new RowDefinition();

        private readonly Border mainBorder = new Border()
        {
            MinHeight = 20,
            MinWidth = 20,
            CornerRadius = new CornerRadius(5),
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.Black,
            Background = Brushes.White,
            Margin = new Thickness(0, 5, 22, 5),
            Padding = new Thickness(3),
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            AllowDrop = true
        };

        private readonly Border leftBorder = new Border() { VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left };
        private readonly Border rightBorder = new Border() { VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left };

        private readonly Canvas leftNodeArrows = new Canvas() { MinHeight = 30, Tag = "Left" };
        private readonly Canvas rightNodeArrows = new Canvas() { MinHeight = 30, Tag = "Right" };
        private readonly Canvas leftNodeInsertPlace = new Canvas() { Width = insertPlaceSize, Height = insertPlaceSize, AllowDrop = true, Tag = "Left" };
        private readonly Canvas rightNodeInsertPlace = new Canvas() { Width = insertPlaceSize, Height = insertPlaceSize, AllowDrop = true, Tag = "Right" };
        private readonly Brush arrowsBrush = new SolidColorBrush(Colors.Black);

        private OperationNode node;
        private OperationNodeVM nodeLeftVM;
        private OperationNodeVM nodeRightVM;

        public OperationNodeVM()
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
            leftNodeInsertPlace.Drop += InsertionDrop;
            rightNodeInsertPlace.Drop += InsertionDrop;

            leftNodeArrows.SizeChanged += NodeArrows_SizeChanged;
            rightNodeArrows.SizeChanged += NodeArrows_SizeChanged;
            AddIntoGrid(leftNodeArrows, 0, 0);
            AddIntoGrid(rightNodeArrows, 1, 0);

            mainBorder.Drop += ReplaceDrop;
            AddIntoGrid(mainBorder, 0, 0, rowSpan: 2);
            AddIntoGrid(leftBorder, 0, 1);
            AddIntoGrid(rightBorder, 1, 1);

            uiElement = grid;
        }

        private void SetNode(OperationNode newNode)
        {
            if (node != null)
            {
                node.PropertyChanged -= Node_PropertyChanged;
            }

            node = newNode;

            if (node != null)
            {
                node.PropertyChanged += Node_PropertyChanged;
            }

            DrawNode();
        }

        private void Node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(node.NodeLeft):
                    if (node.NodeLeft != null)
                    {
                        NodeLeftVM = new OperationNodeVM();
                        leftBorder.Child = NodeLeftVM?.UIElement;
                    }
                    NodeLeftVM.Node = node.NodeLeft;
                    break;
                case nameof(node.NodeRight):
                    if (node.NodeRight != null)
                    {
                        NodeRightVM = new OperationNodeVM();
                        rightBorder.Child = NodeRightVM?.UIElement;
                    }
                    NodeRightVM.Node = node.NodeRight;
                    break;
                case nameof(node.Operation):
                    OperationVM = OperationVM.CreateOperationVM(node, mainBorder);
                    break;
            }
        }

        public void DrawNode()
        {
            OperationVM = OperationVM.CreateOperationVM(node, mainBorder);

            grid.ColumnDefinitions[1].Width = (node == null) ? new GridLength(0) : new GridLength(1, GridUnitType.Auto);
            grid.RowDefinitions[1].Height = (LeftNodeOnlyNedded == true) ? new GridLength(0) : new GridLength(1, GridUnitType.Auto);
            if (node != null)
            {
                leftBorder.Child = NodeLeftVM?.UIElement;
                rightBorder.Child = NodeRightVM?.UIElement;
            }
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

        private void InsertionDrop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.Text) ?? e.Data.GetData(typeof(StructureVariable));
            if (data == null || node == null)
            {
                return;
            }

            var direction = ((FrameworkElement)sender).Tag.ToString();
            var insertNode = CreateNodeFromDrop(data);
            node.InsertNode(insertNode, direction == "Left" ? NodeDirection.Left : NodeDirection.Right);
        }

        private void ReplaceDrop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.Text) ?? e.Data.GetData(typeof(StructureVariable));
            if (data == null)
            {
                return;
            }

            Node.ReplaceNode(CreateNodeFromDrop(data));
            DrawNode();
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

        private void NodeArrows_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Equals(e.NewSize.Width, double.NaN) || Equals(e.NewSize.Height, double.NaN))
            {
                return;
            }

            NodeDirection direction = ((FrameworkElement)sender).Tag.ToString() == "Left" ? NodeDirection.Left : NodeDirection.Right;
            var canvas = direction == NodeDirection.Left ? leftNodeArrows : rightNodeArrows;

            canvas.Children.Clear();

            if (Node.Operation.OperationType == OperationType.Empty)
            {
                return;
            }

            if (direction == NodeDirection.Right && LeftNodeOnlyNedded)
            {
                return;
            }
            
            var directNode = direction == NodeDirection.Left ? Node.NodeLeft : Node.NodeRight;

            double x1 = direction == NodeDirection.Left ? 0 : e.NewSize.Width / 2;
            double x2 = directNode != null ? e.NewSize.Width : e.NewSize.Width - insertPlaceFromRightPosition / 2;

            canvas.Children.Add(new Line
            {
                X1 = x1,
                Y1 = arrowsMarginY,
                X2 = x2,
                Y2 = arrowsMarginY,
                Stroke = arrowsBrush,
                StrokeThickness = arrowsThickness
            });

            if (!LeftNodeOnlyNedded)
            {
                x1 = e.NewSize.Width / 2;
                double y1 = direction == NodeDirection.Left ? arrowsMarginY : 0;
                double y2 = direction == NodeDirection.Left ? e.NewSize.Height : arrowsMarginY;

                canvas.Children.Add(new Line
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x1,
                    Y2 = y2,
                    Stroke = arrowsBrush,
                    StrokeThickness = arrowsThickness
                });
            }

            var insertPlace = direction == NodeDirection.Left ? leftNodeInsertPlace : rightNodeInsertPlace;
            Canvas.SetLeft(insertPlace, e.NewSize.Width - insertPlaceFromRightPosition);
            Canvas.SetTop(insertPlace, insertPlaceFromTopPosition);
            canvas.Children.Add(insertPlace);
        }

        public OperationNode Node
        {
            get => node;
            set => SetNode(value);
        }

        public OperationVM OperationVM { get; private set; }
        public OperationNodeVM NodeLeftVM
        {
            get
            {
                if (nodeLeftVM == null && node?.NodeLeft != null)
                {
                    nodeLeftVM = new OperationNodeVM();
                    nodeLeftVM.Node = node.NodeLeft;
                }

                return nodeLeftVM;
            }
            private set => nodeLeftVM = value; 
        }
        public OperationNodeVM NodeRightVM
        {
            get
            {
                if (nodeRightVM == null && node?.NodeRight != null)
                {
                    nodeRightVM = new OperationNodeVM();
                    nodeRightVM.Node = node.NodeRight;
                }

                return nodeRightVM;
            }
            private set => nodeRightVM = value;
        }

        private UIElement uiElement;
        public UIElement UIElement => uiElement;
        private bool LeftNodeOnlyNedded => node?.Operation?.LeftNodeOnlyNedded ?? true;
    }
}
