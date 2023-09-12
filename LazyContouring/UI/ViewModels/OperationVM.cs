using LazyContouring.Operations;
using LazyPhysicist.Common;
using System.Windows;

namespace LazyContouring.UI.ViewModels
{
    public abstract class OperationVM : Notifier
    {
        protected const double defaultImageWidth = 30;
        protected const double defaultImageHeight = 30;

        public static OperationVM CreateOperationVM(OperationNode node)
        {
            OperationVM result = null;

            if (node == null)
            {
                return new EmptyOperationVM(node);
            }

            switch (node.Operation.OperationType)
            {
                case OperationType.Empty:
                    result = new EmptyOperationVM(node);
                    break;
                case OperationType.Assign:
                    result = new AssignOperationVM(node);
                    break;
                case OperationType.And:
                    result = new AndOperationVM(node);
                    break;
                case OperationType.Or:
                    result = new OrOperationVM(node);
                    break;
                case OperationType.Not:
                    result = new NotOperationVM(node);
                    break;
                case OperationType.Sub:
                    result = new SubOperationVM(node);
                    break;
                case OperationType.Xor:
                    result = new XorOperationVM(node);
                    break;
                case OperationType.Wall:
                    result = new WallOperationVM(node);
                    break;
                case OperationType.Margin:
                    result = new MarginOperationVM(node);
                    break;
                case OperationType.AsymmetricMargin:
                    result = new AsymmetricMarginOperationVM(node);
                    break;
                case OperationType.Crop:
                    result = new CropOperationVM(node);
                    break;
            }

            return result;
        }

        private OperationNode node;

        public OperationVM(OperationNode node)
        {
            Node = node;
            InitUIElement();
        }

        protected abstract void InitUIElement();

        public OperationNode Node { get => node; set => node = value; }
        public UIElement UIElement { get; protected set; }
    }

}
