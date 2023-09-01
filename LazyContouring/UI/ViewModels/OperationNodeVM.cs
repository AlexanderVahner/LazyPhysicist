using LazyContouring.Models;
using LazyContouring.Operations;
using LazyContouring.UI.Views;
using LazyPhysicist.Common;
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
    public sealed class OperationNodeVM : Notifier
    {
        private OperationNode node;
        private OperationNodeVM nodeLeftVM;
        private OperationNodeVM nodeRightVM;

        private UIElement uiElement;

        private UIElement leftNodeElement;
        private UIElement rightNodeElement;
        private UIElement mainElement;
        private OperationVM operationVM;

        public OperationNodeVM() { }

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

            OperationVM = OperationVM.CreateOperationVM(node);

            NodeLeftVM = node?.NodeLeft != null ? new OperationNodeVM() { Node = node.NodeLeft } : null;
            NodeRightVM = node?.NodeRight != null ? new OperationNodeVM() { Node = node.NodeRight } : null;

            DrawNode();

            NotifyPropertyChanged(nameof(Node));
        }

        public void DrawNode()
        {
            MainElement = OperationVM.UIElement;

            LeftNodeElement = NodeLeftVM != null ? new OperationNodeControl() { VM = NodeLeftVM } : null;
            RightNodeElement = NodeRightVM != null ? new OperationNodeControl() { VM = NodeRightVM } : null;

            NotifyPropertyChanged(nameof(LeftNodeOnlyNedded));
        }

        public OperationNode Node { get => node; set => SetNode(value); }

        public OperationVM OperationVM { get => operationVM; private set => SetProperty(ref operationVM, value); }
        public OperationNodeVM NodeLeftVM { get => nodeLeftVM; private set => SetProperty(ref nodeLeftVM, value); }
        public OperationNodeVM NodeRightVM { get => nodeRightVM; private set => SetProperty(ref nodeRightVM, value); }

        public UIElement MainElement { get => mainElement; set => SetProperty(ref mainElement, value); }
        public UIElement LeftNodeElement { get => leftNodeElement; set => SetProperty(ref leftNodeElement, value); }
        public UIElement RightNodeElement { get => rightNodeElement; set => SetProperty(ref rightNodeElement, value); }

        public bool LeftNodeOnlyNedded => node?.Operation?.LeftNodeOnlyNedded ?? true;
        public NodeDirection LeftDirection => NodeDirection.Left;
        public NodeDirection RightDirection => NodeDirection.Right;



        private void Node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(node.NodeLeft):
                    NodeLeftVM = node?.NodeLeft != null ? new OperationNodeVM() { Node = node.NodeLeft } : null;
                    DrawNode();
                    break;
                case nameof(node.NodeRight):
                    NodeRightVM = node?.NodeRight != null ? new OperationNodeVM() { Node = node.NodeRight } : null;
                    DrawNode();
                    break;
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

        public void InsertDrop(IDataObject drop, NodeDirection direction)
        {
            object data = drop.GetData(DataFormats.Text) ?? drop.GetData(typeof(StructureVariable));
            if (data == null || node == null)
            {
                return;
            }

            var insertNode = CreateNodeFromDrop(data);
            node.InsertNode(insertNode, direction);
        }

        public void ReplaceDrop(IDataObject drop, NodeDirection direction)
        {
            object data = drop.GetData(DataFormats.Text) ?? drop.GetData(typeof(StructureVariable));
            if (data == null)
            {
                return;
            }

            Node.ReplaceNode(CreateNodeFromDrop(data), direction);
            DrawNode();
        }
    }
}
