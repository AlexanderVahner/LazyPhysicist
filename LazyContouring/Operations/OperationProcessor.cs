using System.Linq;
using VMS.TPS.Common.Model.API;

namespace LazyContouring.Operations
{
    public sealed class OperationProcessor
    {
        public void Calculate(StructureSet structureSet, OperationNode node)
        {
            if (node == null)
            {
                return;
            }

            Calculate(structureSet, node.NodeLeft);
            Calculate(structureSet, node.NodeRight);

            Perform(structureSet, node);
        }

        private void Perform(StructureSet structureSet, OperationNode node)
        {
            switch (node.Operation.Type)
            {

                case OperationType.Assign:
                    FindOrCreateStructure(structureSet, node);
                    node.StructureVar.SegmentVolume = node.SegmentVolume;
                    break;
                case OperationType.And:
                    node.SegmentVolume = node.NodeLeft.SegmentVolume.And(node.NodeRight.SegmentVolume);
                    break;
                case OperationType.Or:
                    node.SegmentVolume = node.NodeLeft.SegmentVolume.Or(node.NodeRight.SegmentVolume);
                    break;
                case OperationType.Not:
                    node.SegmentVolume = node.NodeLeft.SegmentVolume.Not();
                    break;
                case OperationType.Sub:
                    node.SegmentVolume = node.NodeLeft.SegmentVolume.Sub(node.NodeRight.SegmentVolume);
                    break;
                case OperationType.Xor:
                    node.SegmentVolume = node.NodeLeft.SegmentVolume.Xor(node.NodeRight.SegmentVolume);
                    break;
                case OperationType.Margin:
                    node.SegmentVolume = node.NodeLeft.SegmentVolume.Margin(((MarginOperation)node.Operation).MarginInMM);
                    break;
                case OperationType.AsymmetricMargin:
                    node.SegmentVolume = node.NodeLeft.SegmentVolume.AsymmetricMargin(((AsymmetricMarginOperation)node.Operation).Margins);
                    break;
                case OperationType.Wall:
                    var wall = (WallOperation)node.Operation;
                    node.SegmentVolume = node.NodeLeft.SegmentVolume.Margin(wall.OuterMarginInMM)
                        .Sub(node.NodeLeft.SegmentVolume.Margin(wall.InnerMarginInMM));
                    break;
            }
        }

        private void FindOrCreateStructure(StructureSet structureSet, OperationNode node)
        {
            if (node == null)
            {
                return;
            }

            if (node.StructureVar != null) 
            {
                return;
            }
            
            var findedStructure = structureSet.Structures.FirstOrDefault(s => s.Id == node.StructureVar.StructureId);
            if (findedStructure != null)
            {
                node.StructureVar.Structure = findedStructure;
            }

            if (structureSet.CanAddStructure(node.StructureVar.DicomType, node.StructureVar.StructureId))
            {
                node.StructureVar.Structure = structureSet.AddStructure(node.StructureVar.DicomType, node.StructureVar.StructureId);
            }
        }
    }
}
