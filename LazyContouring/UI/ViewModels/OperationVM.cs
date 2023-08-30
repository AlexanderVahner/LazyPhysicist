using LazyContouring.Images;
using LazyContouring.Models;
using LazyContouring.Operations;
using LazyContouring.UI.Views;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LazyContouring.UI.ViewModels
{
    public abstract class OperationVM
    {
        protected const double defaultImageWidth = 30;
        protected const double defaultImageHeight = 30;
        public static OperationVM CreateOperationVM(OperationNode node, Border border)
        {
            OperationVM result = null;

            if (node == null)
            {
                return new EmptyOperationVM(node, border);
            }

            switch (node.Operation.OperationType)
            {
                case OperationType.Empty:
                    result = new EmptyOperationVM(node, border);
                    break;
                case OperationType.Assign:
                    result = new AssignOperationVM(node, border);
                    break;
                case OperationType.And:
                    result = new AndOperationVM(node, border);
                    break;
                case OperationType.Or:
                    result = new OrOperationVM(node, border);
                    break;
                case OperationType.Not:
                    result = new NotOperationVM(node, border);
                    break;
                case OperationType.Sub:
                    result = new SubOperationVM(node, border);
                    break;
                case OperationType.Xor:
                    result = new XorOperationVM(node, border);
                    break;
                case OperationType.Wall:
                    result = new WallOperationVM(node, border);
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

        private OperationNode node;

        public OperationVM(OperationNode node, Border border)
        {
            SetNode(node);
            ClearBorder(border);
            InitBorder(border);
        }

        private void SetNode(OperationNode node)
        {
            this.node = node;
        }

        protected abstract void InitBorder(Border border);

        private void ClearBorder(Border border)
        {
            border.Child = null;
            border.CornerRadius = new CornerRadius(5);
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Black;
            border.Background = Brushes.White;
            border.Padding = new Thickness(3);
            border.AllowDrop = true;
        }

        public OperationNode Node { get => node; set => SetNode(node); }
    }

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

    public sealed class AssignOperationVM : OperationVM
    {
        public AssignOperationVM(OperationNode node, Border border) : base(node, border) { }

        protected override void InitBorder(Border border)
        {
            border.BorderBrush = new SolidColorBrush(Node?.StructureVar?.Color ?? Color.FromRgb(0, 0, 0));
            border.BorderThickness = new Thickness(3);
            border.Child = new TextBlock
            {
                Text = (Node?.StructureVar?.StructureId ?? "*drop structure here*") + " = ",
            };
        }
    }

    public sealed class AndOperationVM : OperationVM
    {
        public AndOperationVM(OperationNode node, Border border) : base(node, border) { }

        protected override void InitBorder(Border border)
        {
            border.Child = new Image()
            {
                Width = defaultImageWidth,
                Height = defaultImageHeight,
                Source = ImageLoader.GetImage("AndOperation.png")
            };
        }
    }

    public sealed class OrOperationVM : OperationVM
    {
        public OrOperationVM(OperationNode node, Border border) : base(node, border) { }

        protected override void InitBorder(Border border)
        {
            border.Child = new Image()
            {
                Width = defaultImageWidth,
                Height = defaultImageHeight,
                Source = ImageLoader.GetImage("OrOperation.png")
            };
        }
    }

    public sealed class NotOperationVM : OperationVM
    {
        public NotOperationVM(OperationNode node, Border border) : base(node, border) { }

        protected override void InitBorder(Border border)
        {
            border.Child = new Image()
            {
                Width = defaultImageWidth,
                Height = defaultImageHeight,
                Source = ImageLoader.GetImage("NotOperation.png")
            };
        }
    }

    public sealed class SubOperationVM : OperationVM
    {
        public SubOperationVM(OperationNode node, Border border) : base(node, border) { }

        protected override void InitBorder(Border border)
        {
            border.Child = new Image() { 
                Width = defaultImageWidth, 
                Height = defaultImageHeight, 
                Source = ImageLoader.GetImage("SubOperation.png") };
        }
    }

    public sealed class XorOperationVM : OperationVM
    {
        public XorOperationVM(OperationNode node, Border border) : base(node, border) { }

        protected override void InitBorder(Border border)
        {
            border.Child = new Image()
            {
                Width = defaultImageWidth,
                Height = defaultImageHeight,
                Source = ImageLoader.GetImage("XorOperation.png")
            };
        }
    }

    public sealed class WallOperationVM : OperationVM
    {
        public WallOperationVM(OperationNode node, Border border) : base(node, border) { }

        protected override void InitBorder(Border border)
        {
            var wallControl = new WallOperationControl() { DataContext = Node.Operation as WallOperation };
            border.Child = wallControl;
        }
    }

}
