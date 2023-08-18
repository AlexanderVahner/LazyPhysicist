using LazyContouring.Models;
using LazyContouring.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LazyContouring.UI.ViewModels
{
    public abstract class OperationVM
    {
        public static OperationVM CreateOperationVM(OperationNode node)
        {
            OperationVM result = null;

            switch (node.Operation.OperationType)
            {
                case OperationType.Empty:
                    result = new EmptyOperationVM(node);
                    break;
                case OperationType.Assign:
                    result = new AssignOperationVM(node);
                    break;
                case OperationType.And:
                    break;
                case OperationType.Or:
                    break;
                case OperationType.Not:
                    break;
                case OperationType.Sub:
                    result = new SubOperationVM(node);
                    break;
                case OperationType.Xor:
                    break;
                case OperationType.Wall:
                    break;
                case OperationType.Margin:
                    break;
                case OperationType.AsymmetricMargin:
                    break;
                case OperationType.Crop:
                    break;
            }

            return result;
        }

        protected Border CreateDefaultBorder()
        {
            return new Border
            {
                CornerRadius = new CornerRadius(5),
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Background = Brushes.White,
                Margin = new Thickness(5),
                Padding = new Thickness(3),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left
            };
        }

        protected Border CreateDefaultStructureBorder(StructureVariable structureVar)
        {
            var border = CreateDefaultBorder();
            border.BorderBrush = new SolidColorBrush(structureVar?.Structure?.Color ?? Color.FromRgb(0, 0, 0));
            border.BorderThickness = new Thickness(3);
            border.Child = new TextBlock
            {
                Text = structureVar?.StructureId ?? "Drop structure here",
            };
            return border;
        }

        private OperationNode node;

        public OperationVM(OperationNode node)
        {
            SetNode(node);
        }

        private void SetNode(OperationNode node)
        {
            this.node = node;
            CreateUI();
        }

        protected abstract void CreateUI();

        public OperationNode Node { get => node; set => SetNode(node); }
        public UIElement UIElement { get; set; }
    }

    public sealed class EmptyOperationVM : OperationVM
    {
        public EmptyOperationVM(OperationNode node) : base(node) { }

        protected override void CreateUI()
        {
            var structureBorder = CreateDefaultStructureBorder(Node.StructureVar);
            UIElement = structureBorder;
        }
    }

    public sealed class AssignOperationVM : OperationVM
    {
        public AssignOperationVM(OperationNode node) : base(node) { }

        protected override void CreateUI()
        {
            var structureBorder = CreateDefaultStructureBorder(Node.StructureVar);

            var operationBorder = CreateDefaultBorder();
            operationBorder.Child = new TextBlock 
            { 
                Text = "=",
                FontWeight = FontWeights.Bold
            };

            var stackPanel = new StackPanel 
            { 
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            stackPanel.Children.Add(structureBorder);
            stackPanel.Children.Add(operationBorder);

            UIElement = stackPanel;
        }
    }

    public sealed class SubOperationVM : OperationVM
    {
        public SubOperationVM(OperationNode node) : base(node) { }

        protected override void CreateUI()
        {
            var border = CreateDefaultBorder();
            border.Child = new TextBlock { Text = "Sub" };
            UIElement = border;
        }
    }

}
