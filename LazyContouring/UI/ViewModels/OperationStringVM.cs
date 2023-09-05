using LazyContouring.Operations;
using LazyContouring.UI.Views;
using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LazyContouring.UI.ViewModels
{
    public sealed class OperationStringVM : Notifier
    {
        private OperationNode node;
        private UIElement nodeElement;
        private readonly Grid grid = new Grid();

        public OperationStringVM() { }

        private void SetNode(OperationNode node)
        {
            if (node == null)
            {
                return;
            }
            this.node = node;
            var nodeVM = new OperationNodeVM() { Node = node };
            var nodeUI = new OperationNodeControl() { VM = nodeVM };

            NodeElement = nodeUI;
            NotifyPropertyChanged(nameof(Node));
        }


        public OperationNode Node { get => node; set => SetNode(value); }
        public UIElement NodeElement { get => nodeElement; set => SetProperty(ref nodeElement, value); }
    }
}
