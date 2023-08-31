using LazyContouring.Models;
using LazyPhysicist.Common;
using System.Linq;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Operations
{
    public enum NodeDirection { Left, Right };
    public sealed class OperationNode : Notifier
    {
        private SegmentVolume segmentVolume;
        private StructureVariable structureVar;
        private Operation operation;
        private OperationNode nodeLeft;
        private OperationNode nodeRight;

        private void Materialize(StructureSet structureSet)
        {
            NodeLeft?.Materialize(structureSet);
            NodeRight?.Materialize(structureSet);

            if (Operation.StructureNedded)
            {
                FindOrCreateStructure(structureSet);
            }

            Operation.Perform(this);
        }

        public void InsertNodeBefore(OperationNode insertNode, ref OperationNode beforeThisNode)
        {
            insertNode.NodeLeft = beforeThisNode;
            beforeThisNode = insertNode;
        }

        public void InsertNode(OperationNode newNode, NodeDirection direction)
        {
            if (newNode == null)
            {
                return;
            }

            if (newNode.Operation.OperationType != OperationType.Empty)
            {
                newNode.NodeLeft = direction == NodeDirection.Left ? NodeLeft : NodeRight;
            }
            
            if (direction == NodeDirection.Left)
            {
                NodeLeft = newNode;
            }
            else
            {
                NodeRight = newNode;
            }
        }

        public void ReplaceNode(OperationNode newNode)
        {
            if (newNode == null)
            {
                return;
            }

            Operation = newNode.Operation;
            StructureVar = newNode.StructureVar;
            newNode.NodeLeft = NodeLeft;
            if (newNode.Operation.LeftNodeOnlyNedded)
            {
                NodeRight = null;
            }
            else
            {
                newNode.NodeRight = NodeRight;
            }
            NotifyPropertyChanged(nameof(NodeLeft));
            NotifyPropertyChanged(nameof(NodeRight));
        }

        public void DeleteNode(NodeDirection direction)
        {
            if (direction == NodeDirection.Left)
            {
                NodeLeft = null;
            }
            else
            {
                NodeRight = null;
            }
        }

        public void FindOrCreateStructure(StructureSet structureSet)
        {
            if (StructureVar == null)
            {
                StructureVar = new StructureVariable();
            }
            else if (StructureVar.Structure != null)
            {
                return;
            }

            var findedStructure = structureSet.Structures.FirstOrDefault(s => s.Id == StructureVar.StructureId);
            if (findedStructure != null)
            {
                StructureVar.Structure = findedStructure;
            }

            if (structureSet.CanAddStructure(StructureVar.DicomType, StructureVar.StructureId))
            {
                StructureVar.Structure = structureSet.AddStructure(StructureVar.DicomType, StructureVar.StructureId);
            }
        }

        public SegmentVolume SegmentVolume
        {
            get => segmentVolume ?? StructureVar?.SegmentVolume;
            set => segmentVolume = value;
        }

        public bool IsRootNode { get; set; } = false;
        public StructureVariable StructureVar { get => structureVar; set => SetProperty(ref structureVar, value); }
        public Operation Operation { get => operation; set => SetProperty(ref operation, value); }
        public OperationNode NodeLeft { get => nodeLeft; set => SetProperty(ref nodeLeft, value); }
        public OperationNode NodeRight { get => nodeRight; set => SetProperty(ref nodeRight, value); }
    }
}
