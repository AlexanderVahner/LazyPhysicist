using LazyContouring.Models;
using LazyContouring.Operations;
using LazyPhysicist.Common;
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

namespace LazyContouring.UI.ViewModels
{
    public abstract class OperationVM : Notifier
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
                    result = new MarginOperationVM(node, border);
                    break;
                case OperationType.AsymmetricMargin:
                    result = new AsymmetricMarginOperationVM(node, border);
                    break;
                case OperationType.Crop:
                    result = new CropOperationVM(node, border);
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

}
