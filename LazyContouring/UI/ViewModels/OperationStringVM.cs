using LazyContouring.Images;
using LazyContouring.Operations;
using LazyContouring.UI.Views;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace LazyContouring.UI.ViewModels
{
    public sealed class OperationStringVM : Notifier
    {
        private OperationNode node;
        private UIElement nodeElement;
        private AssignOperation assignOperation;
        private BitmapImage executeButtonImage;
        private readonly BitmapImage executeImage = ImageLoader.GetImage("Ionic-Ionicons-Caret-forward-circle.512.png");
        private readonly BitmapImage undoExecuteImage = ImageLoader.GetImage("Ionic-Ionicons-Arrow-undo-circle-outline.512.png");
        private readonly Grid grid = new Grid();

        public OperationStringVM() { }

        private void SetNode(OperationNode node)
        {
            if (node == null)
            {
                return;
            }
            this.node = node;
            assignOperation = (AssignOperation)node.Operation;
            ExecuteButtonImage = executeImage;
            var nodeVM = new OperationNodeVM() { Node = node };
            var nodeUI = new OperationNodeControl() { VM = nodeVM };

            NodeElement = nodeUI;
            NotifyPropertyChanged(nameof(Node));
        }

        public void Execute()
        {
            if (Executed)
            {
                assignOperation.Undo(Node);
                ExecuteButtonImage = executeImage;
            }
            else if (CanExecute)
            {
                Node.Materialize(null);
                ExecuteButtonImage = undoExecuteImage;
            }
        }

        public MetaCommand ExecuteCommand => new MetaCommand(
            o => Execute(),
            o => CanExecute
        );

        public bool CanExecute => Executed || (assignOperation?.CanExecute(node) ?? false);
        public bool Executed => assignOperation?.Executed ?? false;
        public BitmapImage ExecuteButtonImage { get => executeButtonImage; set => SetProperty(ref executeButtonImage, value); }
        public OperationNode Node { get => node; set => SetNode(value); }
        public UIElement NodeElement { get => nodeElement; set => SetProperty(ref nodeElement, value); }
    }
}
