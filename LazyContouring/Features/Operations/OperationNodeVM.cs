using LazyContouring.Models;
using LazyContouring.Operations;
using LazyPhysicist.Common;
using System;
using System.Windows;
using System.Windows.Media;

namespace LazyContouring.UI.ViewModels
{
    public sealed class OperationNodeVM : Notifier
    {
        private OperationNode node;
        private OperationNodeVM nodeLeftVM;
        private OperationNodeVM nodeRightVM;
        private OperationVM operationVM;
        private StructureVariableVM structureVarVM;

        private Thickness mainBorderThickness;
        private Brush borderBrush;
        private Visibility removeDropPlaceVisibility = Visibility.Collapsed;

        private readonly Brush defaultBorderBrush = new SolidColorBrush(Colors.Black);
        private readonly Thickness defaultBorderThickness = new Thickness(1);
        private readonly Thickness borderThicknessWihtStructure = new Thickness(3);


        public OperationNodeVM()
        {
            mainBorderThickness = defaultBorderThickness;
        }

        private void SetNode(OperationNode newNode)
        {
            if (node != null) { node.PropertyChanged -= Node_PropertyChanged; }
            node = newNode;
            if (node != null) { node.PropertyChanged += Node_PropertyChanged; }

            OperationVM = OperationVM.CreateOperationVM(node);

            NodeLeftVM = node?.NodeLeft != null ? new OperationNodeVM() { Node = node.NodeLeft } : null;
            NodeRightVM = node?.NodeRight != null ? new OperationNodeVM() { Node = node.NodeRight } : null;

            StructureVarVM = node?.StructureVar != null ? new StructureVariableVM(node?.StructureVar) : null;
            BorderBrush = StructureVarVM?.StructureVariable != null ? StructureVarVM.StrokeBrush : defaultBorderBrush;
            MainBorderThickness = StructureVarVM?.StructureVariable != null ? borderThicknessWihtStructure : defaultBorderThickness;

            NotifyPropertyChanged(nameof(Node));
            NotifyPropertyChanged(nameof(LeftNodeNedded));
            NotifyPropertyChanged(nameof(RightNodeNedded));
            NotifyPropertyChanged(nameof(BorderBrush));
            NotifyPropertyChanged(nameof(MainBorderThickness));
        }

        private void Node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(node.Operation):
                    OperationVM = OperationVM.CreateOperationVM(node);
                    break;
                case nameof(node.NodeLeft):
                    NodeLeftVM = node?.NodeLeft != null ? new OperationNodeVM() { Node = node.NodeLeft } : null;
                    break;
                case nameof(node.NodeRight):
                    NodeRightVM = node?.NodeRight != null ? new OperationNodeVM() { Node = node.NodeRight } : null;
                    break;
                case nameof(node.StructureVar):
                    StructureVarVM.StructureVariable = node.StructureVar;
                    BorderBrush = StructureVarVM.StructureVariable != null ? StructureVarVM.StrokeBrush : defaultBorderBrush;
                    MainBorderThickness = StructureVarVM.StructureVariable != null ? borderThicknessWihtStructure : defaultBorderThickness;
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

        public void ReplaceDrop(IDataObject drop)
        {
            object data = drop.GetData(DataFormats.Text) ?? drop.GetData(typeof(StructureVariable));
            if (data == null)
            {
                return;
            }

            var node = CreateNodeFromDrop(data);

            if (Node.Operation.OperationType == OperationType.Assign && node.StructureVar != null)
            {
                Node.StructureVar = node.StructureVar;
            }
            else
            {
                ReplaceNeeded?.Invoke(this, node); // Push to parent
            }
        }

        public void PerformReplaceLeft(object sender, OperationNode e)
        {
            Node.ReplaceNode(e, NodeDirection.Left);
        }

        public void PerformReplaceRight(object sender, OperationNode e)
        {
            Node.ReplaceNode(e, NodeDirection.Right);
        }

        private void StructureVarVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(StructureVarVM.StructureVariable):
                    MainBorderThickness = StructureVarVM.StructureVariable != null ? borderThicknessWihtStructure : defaultBorderThickness;
                    break;
                case nameof(structureVarVM.StrokeBrush):
                    BorderBrush = StructureVarVM.StructureVariable != null ? StructureVarVM.StrokeBrush : defaultBorderBrush;
                    break;
            }
        }

        public EventHandler<OperationNode> ReplaceNeeded;

        public OperationNode Node { get => node; set => SetNode(value); }
        public OperationVM OperationVM { get => operationVM; private set => SetProperty(ref operationVM, value); }
        public OperationNodeVM NodeLeftVM
        {
            get => nodeLeftVM;
            private set
            {
                if (nodeLeftVM != null) { nodeLeftVM.ReplaceNeeded -= PerformReplaceLeft; }
                SetProperty(ref nodeLeftVM, value);
                if (nodeLeftVM != null) { nodeLeftVM.ReplaceNeeded += PerformReplaceLeft; }
            }
        }
        public OperationNodeVM NodeRightVM
        {
            get => nodeRightVM;
            private set
            {
                if (nodeRightVM != null) { nodeRightVM.ReplaceNeeded -= PerformReplaceRight; }
                SetProperty(ref nodeRightVM, value);
                if (nodeRightVM != null) { nodeRightVM.ReplaceNeeded += PerformReplaceRight; }
            }
        }
        public StructureVariableVM StructureVarVM
        {
            get => structureVarVM;
            set
            {
                if (structureVarVM != null) { structureVarVM.PropertyChanged -= StructureVarVM_PropertyChanged; }
                SetProperty(ref structureVarVM, value);
                if (structureVarVM != null) { structureVarVM.PropertyChanged += StructureVarVM_PropertyChanged; }
            }
        }



        public bool LeftNodeNedded => node?.Operation?.LeftNodeNedded ?? false;
        public bool RightNodeNedded => node?.Operation?.RightNodeNedded ?? false;
        public NodeDirection LeftDirection => NodeDirection.Left;
        public NodeDirection RightDirection => NodeDirection.Right;

        public Thickness MainBorderThickness { get => mainBorderThickness; set => SetProperty(ref mainBorderThickness, value); }
        public Brush BorderBrush { get => borderBrush; set => SetProperty(ref borderBrush, value); }
        public Visibility RemoveDropPlaceVisibility { get => removeDropPlaceVisibility; set => SetProperty(ref removeDropPlaceVisibility, value); }
    }
}
